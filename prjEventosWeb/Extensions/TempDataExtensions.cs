using Microsoft.AspNetCore.Mvc;

namespace prjEventosWeb.Extensions
{
    public static class TempDataExtensions
    {
        /// <summary>
        /// Adiciona uma mensagem de sucesso ao TempData
        /// </summary>
        public static void AddSucesso(this Controller controller, string mensagem)
        {
            controller.TempData["Sucesso"] = mensagem;
        }

        /// <summary>
        /// Adiciona uma mensagem de erro ao TempData
        /// </summary>
        public static void AddErro(this Controller controller, string mensagem)
        {
            controller.TempData["Erro"] = mensagem;
        }

        /// <summary>
        /// Adiciona uma mensagem de aviso ao TempData
        /// </summary>
        public static void AddAviso(this Controller controller, string mensagem)
        {
            controller.TempData["Aviso"] = mensagem;
        }

        /// <summary>
        /// Adiciona uma mensagem sobre email de sucesso ao TempData
        /// </summary>
        public static void AddEmailSucesso(this Controller controller, string mensagem)
        {
            controller.TempData["EmailSucesso"] = mensagem;
        }

        /// <summary>
        /// Adiciona uma mensagem sobre erro de email ao TempData
        /// </summary>
        public static void AddEmailErro(this Controller controller, string mensagem)
        {
            controller.TempData["EmailErro"] = mensagem;
        }
    }
}