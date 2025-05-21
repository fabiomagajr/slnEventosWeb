using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prjEventosWeb.Data;
using prjEventosWeb.Extensions;
using prjEventosWeb.Models;
using prjEventosWeb.Services;

namespace prjEventosWeb.Controllers
{
    public class InscricaoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public InscricaoController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Garante que o usuário esteja logado
        public async Task<IActionResult> Inscrever(int eventoId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Challenge(); // Ou redirecionar para login
            }

            // Tenta usar uma transação para garantir a atomicidade da verificação e inserção
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. Buscar o evento incluindo as inscrições para contá-las
                    var evento = await _context.Evento
                                               .Include(e => e.Inscricoes)
                                               .FirstOrDefaultAsync(e => e.Id == eventoId);

                    if (evento == null)
                    {
                        this.AddErro("Evento não encontrado.");
                        return RedirectToAction("Index", "Evento");
                    }

                    // 2. Verificar se o usuário já está inscrito neste evento
                    bool jaInscrito = await _context.Inscricao
                                                    .AnyAsync(i => i.EventoId == eventoId && i.ApplicationUserId == userId);
                    if (jaInscrito)
                    {
                        this.AddAviso("Você já está inscrito neste evento.");
                        return RedirectToAction("Details", "Evento", new { id = eventoId });
                    }

                    // 3. Verificar se ainda há vagas disponíveis
                    if (evento.VagasDisponiveis <= 0)
                    {
                        this.AddErro("Não há mais vagas disponíveis para este evento.");
                        await transaction.RollbackAsync();
                        return RedirectToAction("Details", "Evento", new { id = eventoId });
                    }

                    // 4. Se tudo estiver OK, criar a inscrição
                    var novaInscricao = new Inscricao
                    {
                        EventoId = eventoId,
                        ApplicationUserId = userId,
                        DataInscricao = DateTime.UtcNow
                    };

                    _context.Inscricao.Add(novaInscricao);
                    await _context.SaveChangesAsync(); // Salva a nova inscrição

                    await transaction.CommitAsync(); // Confirma a transação

                    // 5. Enviar email de confirmação
                    var usuario = await _userManager.FindByIdAsync(userId);
                    bool emailEnviado = await _emailService.EnviarEmailConfirmacaoInscricao(usuario, evento);

                    // Se o email foi enviado, adicionar mensagem de sucesso adicional
                    this.AddSucesso($"Inscrição no evento '{evento.Nome}' realizada com sucesso!");
                    if (emailEnviado)
                    {
                        this.AddEmailSucesso($"Um email de confirmação foi enviado para {usuario.Email}.");
                    }
                    else
                    {
                        this.AddEmailErro("Não foi possível enviar o email de confirmação, mas sua inscrição foi realizada.");
                    }

                    return RedirectToAction("MinhasInscricoes");
                }
                catch (DbUpdateException ex)
                {
                    await transaction.RollbackAsync();
                    this.AddErro("Ocorreu um erro ao tentar se inscrever. Verifique se você já não está inscrito ou tente novamente.");
                    return RedirectToAction("Details", "Evento", new { id = eventoId });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    this.AddErro("Ocorreu um erro inesperado ao processar sua inscrição.");
                    return RedirectToAction("Index", "Evento");
                }
            }
        }

        // Action para o aluno ver suas inscrições
        [Authorize]
        public async Task<IActionResult> MinhasInscricoes()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var inscricoes = await _context.Inscricao
                                    .Where(i => i.ApplicationUserId == userId)
                                    .Include(i => i.Evento) // Para mostrar detalhes do evento
                                        .ThenInclude(e => e.Categoria) // E da categoria do evento
                                    .ToListAsync();

            return View(inscricoes);
        }

        // GET: Inscricao
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Inscricao.Include(i => i.Evento);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Inscricao/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inscricao = await _context.Inscricao
                .Include(i => i.Evento)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inscricao == null)
            {
                return NotFound();
            }

            return View(inscricao);
        }

        // GET: Inscricao/Create
        public IActionResult Create()
        {
            ViewData["EventoId"] = new SelectList(_context.Evento, "Id", "Id");
            return View();
        }

        // POST: Inscricao/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Email,DataInscricao,EventoId")] Inscricao inscricao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inscricao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventoId"] = new SelectList(_context.Evento, "Id", "Id", inscricao.EventoId);
            return View(inscricao);
        }

        // GET: Inscricao/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inscricao = await _context.Inscricao.FindAsync(id);
            if (inscricao == null)
            {
                return NotFound();
            }
            ViewData["EventoId"] = new SelectList(_context.Evento, "Id", "Id", inscricao.EventoId);
            return View(inscricao);
        }

        // POST: Inscricao/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email,DataInscricao,EventoId")] Inscricao inscricao)
        {
            if (id != inscricao.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inscricao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InscricaoExists(inscricao.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventoId"] = new SelectList(_context.Evento, "Id", "Id", inscricao.EventoId);
            return View(inscricao);
        }

        // GET: Inscricao/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inscricao = await _context.Inscricao
                .Include(i => i.Evento)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inscricao == null)
            {
                return NotFound();
            }

            return View(inscricao);
        }

        // POST: Inscricao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inscricao = await _context.Inscricao.FindAsync(id);
            if (inscricao != null)
            {
                _context.Inscricao.Remove(inscricao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InscricaoExists(int id)
        {
            return _context.Inscricao.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Cancelar(int inscricaoId)
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Challenge();

            // Usando transação para garantir atomicidade
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Buscar a inscrição garantindo que pertença ao usuário atual
                    var inscricao = await _context.Inscricao
                                        .Include(i => i.Evento)
                                        .FirstOrDefaultAsync(i => i.Id == inscricaoId && i.ApplicationUserId == userId);

                    if (inscricao == null)
                    {
                        this.AddErro("Inscrição não encontrada ou você não tem permissão para cancelá-la.");
                        return RedirectToAction("MinhasInscricoes");
                    }

                    // Guarda o ID do evento para redirecionar depois
                    int eventoId = inscricao.EventoId;
                    string nomeEvento = inscricao.Evento?.Nome ?? "evento";

                    // Remove a inscrição
                    _context.Inscricao.Remove(inscricao);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    this.AddSucesso($"Sua inscrição no evento '{nomeEvento}' foi cancelada com sucesso.");
                    return RedirectToAction("MinhasInscricoes");
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    this.AddErro("Ocorreu um erro ao cancelar sua inscrição.");
                    return RedirectToAction("MinhasInscricoes");
                }
            }
        }
    }
}