using kidzkonnect2._0.Models;
using Microsoft.EntityFrameworkCore;
using IO.Ably; // Importation du package Ably
using IO.Ably.Realtime; // Importation des biblioth�ques n�cessaires

var builder = WebApplication.CreateBuilder(args);

// Initialisation d'Ably avec votre cl� API
var ably = new AblyRealtime("u7sjCg.kolkvQ:mA9tLK7D27Pejjtr2HSAyAnENtCuKYHK48RwCtI-H2c");

// R�cup�ration de la cha�ne de connexion � la base de donn�es depuis la configuration
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// Ajout des services au conteneur.
builder.Services.AddControllersWithViews();

// Enregistrement de AblyRealtime comme service singleton
builder.Services.AddSingleton<AblyRealtime>(ably);

// Configuration du contexte de base de donn�es pour Entity Framework Core
builder.Services.AddDbContext<kidzkonnectContext>(options =>
{
    options.UseSqlServer(connection);
});

// Ajout des services de session
builder.Services.AddDistributedMemoryCache(); // Ajout du cache en m�moire distribu�e
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // D�finition du d�lai d'expiration de la session
    options.Cookie.HttpOnly = true; // Marquage du cookie de session comme accessible uniquement via HTTP
    options.Cookie.IsEssential = true; // Rendre le cookie de session essentiel
});

var app = builder.Build(); // Construction de l'application

// Configuration du pipeline de requ�tes HTTP.
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

// D�finition de la route par d�faut pour les contr�leurs
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=SignInUp}/{id?}");

// Connexion � Ably avec votre cl� API
ably.Connection.On(ConnectionEvent.Connected, args =>
{
    Console.Out.WriteLine("Connect� � Ably !");
});

// Cr�ation d'un canal appel� 'chat' et enregistrement d'un �couteur pour s'abonner � tous les messages avec le nom 'first'
var channel = ably.Channels.Get("chat");
channel.Subscribe("first", message =>
{
    Console.Out.WriteLine("Message re�u : {0}", message.Data);
});

// Publication d'un message avec le nom 'first' et le contenu 'Voici mon premier message !'
channel.Publish("first", "Voici mon premier message !");

// Fermeture de la connexion � Ably
ably.Connection.Close();
ably.Connection.On(ConnectionEvent.Closed, args =>
{
    Console.Out.WriteLine("Connexion � Ably ferm�e.");
});

app.Run(); // Lancement de l'application
