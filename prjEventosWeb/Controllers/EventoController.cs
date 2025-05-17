using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prjEventosWeb.Data;
using prjEventosWeb.Models;

namespace prjEventosWeb.Controllers
{
    public class EventoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventoController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Evento
        public async Task<IActionResult> Index(int? categoriaId = null)
        {
            DateTime dataAtual = DateTime.Now;

            // Query base
            var query = _context.Evento
                .Include(e => e.Categoria)
                .Where(e => e.DataInicio >= dataAtual);

            // Adiciona filtro por categoria se o parâmetro foi fornecido
            if (categoriaId.HasValue)
            {
                query = query.Where(e => e.CategoriaId == categoriaId.Value);
                // Opcional: armazenar o nome da categoria para exibir na view
                ViewBag.CategoriaFiltrada = await _context.Categoria
                    .Where(c => c.Id == categoriaId.Value)
                    .Select(c => c.Nome)
                    .FirstOrDefaultAsync();
            }

            // Ordenação e execução da query
            var eventos = await query
                .OrderBy(e => e.DataInicio)
                .ToListAsync();

            return View(eventos);
        }

        // GET: Evento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Aqui está a correção: Incluir explicitamente as Inscrições no carregamento do Evento
            var evento = await _context.Evento
                .Include(e => e.Categoria)
                .Include(e => e.Inscricoes)  // Adicionada esta linha para carregar as inscrições
                .FirstOrDefaultAsync(m => m.Id == id);

            if (evento == null)
            {
                return NotFound();
            }

            ViewBag.UserId = _userManager.GetUserId(User);

            // Para debug: Verificar quantidade de inscritos e vagas disponíveis
            ViewBag.NumeroInscricoes = evento.Inscricoes?.Count() ?? 0;
            ViewBag.VagasDisponiveis = evento.VagasDisponiveis;

            return View(evento);
        }

        // GET: Evento/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Nome"); // Alterado para exibir o Nome da categoria em vez do Id
            return View();
        }

        // POST: Evento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Apresentador,UrlImgApresentador,Descricao,DataInicio,DataFim,Local,Preco,LimiteVagas,ImagemUrl,CategoriaId")] Evento evento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(evento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Nome", evento.CategoriaId); // Alterado para exibir o Nome da categoria
            return View(evento);
        }

        // GET: Evento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Evento.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Nome", evento.CategoriaId); // Alterado para exibir o Nome da categoria
            return View(evento);
        }

        // POST: Evento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Apresentador,UrlImgApresentador,Descricao,DataInicio,DataFim,Local,Preco,LimiteVagas,ImagemUrl,CategoriaId")] Evento evento)
        {
            if (id != evento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventoExists(evento.Id))
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
            ViewData["CategoriaId"] = new SelectList(_context.Categoria, "Id", "Nome", evento.CategoriaId); // Alterado para exibir o Nome da categoria
            return View(evento);
        }

        // GET: Evento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Evento
                .Include(e => e.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        // POST: Evento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evento = await _context.Evento.FindAsync(id);
            if (evento != null)
            {
                _context.Evento.Remove(evento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventoExists(int id)
        {
            return _context.Evento.Any(e => e.Id == id);
        }
    }
}