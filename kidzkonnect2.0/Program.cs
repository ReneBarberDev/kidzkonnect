using kidzkonnect2._0.Models;
using Microsoft.EntityFrameworkCore;
using IO.Ably; // Importation du package Ably
using IO.Ably.Realtime; // Importation des bibliothèques nécessaires

var builder = WebApplication.CreateBuilder(args);

// Initialisation d'Ably avec votre clé API
var ably = new AblyRealtime("u7sjCg.kolkvQ:mA9tLK7D27Pejjtr2HSAyAnENtCuKYHK48RwCtI-H2c");

// Récupération de la chaîne de connexion à la base de données depuis la configuration
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// Ajout des services au conteneur.
builder.Services.AddControllersWithViews();

// Enregistrement de AblyRealtime comme service singleton
builder.Services.AddSingleton<AblyRealtime>(ably);

// Configuration du contexte de base de données pour Entity Framework Core
builder.Services.AddDbContext<kidzkonnectContext>(options =>
{
    options.UseSqlServer(connection);
});

// Ajout des services de session
builder.Services.AddDistributedMemoryCache(); // Ajout du cache en mémoire distribuée
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Définition du délai d'expiration de la session
    options.Cookie.HttpOnly = true; // Marquage du cookie de session comme accessible uniquement via HTTP
    options.Cookie.IsEssential = true; // Rendre le cookie de session essentiel
});

var app = builder.Build(); // Construction de l'application

// Configuration du pipeline de requêtes HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Gestion des exceptions en cas d'erreur
    app.UseHsts(); // Activation de HSTS (HTTP Strict Transport Security)
}

app.UseHttpsRedirection(); // Redirection HTTP vers HTTPS
app.UseStaticFiles(); // Activation des fichiers statiques

app.UseSession(); // Activation de la gestion de session

app.UseRouting(); // Activation du routage

app.UseAuthentication(); // Activation de l'authentification
app.UseAuthorization(); // Activation de l'autorisation

// Définition de la route par défaut pour les contrôleurs
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=SignInUp}/{id?}");

// Connexion à Ably avec votre clé API
ably.Connection.On(ConnectionEvent.Connected, args =>
{
    Console.Out.WriteLine("Connecté à Ably !");
});

// Création d'un canal appelé 'chat' et enregistrement d'un écouteur pour s'abonner à tous les messages avec le nom 'first'
var channel = ably.Channels.Get("chat");
channel.Subscribe("first", message =>
{
    Console.Out.WriteLine("Message reçu : {0}", message.Data);
});

// Publication d'un message avec le nom 'first' et le contenu 'Voici mon premier message !'
channel.Publish("first", "Voici mon premier message !");

// Fermeture de la connexion à Ably
ably.Connection.Close();
ably.Connection.On(ConnectionEvent.Closed, args =>
{
    Console.Out.WriteLine("Connexion à Ably fermée.");
});

app.Run(); // Lancement de l'application
