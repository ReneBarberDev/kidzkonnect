using kidzkonnect2._0.Models; // Importation du modèle de données
using Microsoft.AspNetCore.Mvc; // Importation des fonctionnalités de MVC
using System.Diagnostics; // Importation de la bibliothèque de diagnostics

namespace kidzkonnect2._0.Controllers // Déclaration de l'espace de noms du contrôleur
{
    public class HomeController : Controller // Déclaration de la classe HomeController, qui hérite de Controller
    {
        private readonly ILogger<HomeController> _logger; // Déclaration d'un champ privé pour le journal des événements

        public HomeController(ILogger<HomeController> logger) // Constructeur de la classe HomeController, injectant un ILogger<HomeController>
        {
            _logger = logger; // Initialisation du champ privé _logger avec l'instance de ILogger fournie
        }

        public IActionResult Index() // Action pour afficher la page d'accueil
        {
            return View(); // Renvoie la vue correspondant à la page d'accueil
        }

        public IActionResult SignInUp() // Action pour afficher la page de connexion ou inscription
        {
            return View(); // Renvoie la vue correspondant à la page de connexion ou inscription
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] // Configuration de la mise en cache de la réponse
        public IActionResult Error() // Action pour gérer les erreurs
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }); // Renvoie la vue d'erreur avec les informations sur l'erreur
        }
    }
}
