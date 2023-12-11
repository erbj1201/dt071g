using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using static moment_3.Guestbook;

namespace moment_3
{
    class GuestbookPost
    {
        private string message = string.Empty; // Meddelande för gästboksinlägg
        private string author = string.Empty;   // Författare av gästboksinlägg
        private DateTime timestamp;  // Tidpunkt då inlägget skapades
        private static int nextId = 1; // Statisk variabel för att hålla koll på nästa tillgängliga ID

        // Egenskaper med get- och set-metoder med hantering av null-värden
        public string Message
        {
            get { return message; }
            set { message = value ?? string.Empty; }
        }
        public string Author
        {
            get { return author; }
            set { author = value ?? string.Empty; }
        }
        public DateTime Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }
        public int PostId { get; }

        // Konstruktor
        public GuestbookPost(string? message, string? author)
        {
            this.message = message ?? string.Empty; // Tilldelning av meddelande med hantering av null-värde
            this.Author = author ?? string.Empty;    // Tilldelning av författare med hantering av null-värde
            this.Timestamp = DateTime.Now;           // Sätter tidpunkten till aktuellt datum och tid
            this.PostId = nextId++; // Tilldela det aktuella värdet av nextId och öka sedan värdet
        }
        // Metod för att uppdatera nästa tillgängliga ID
        public static void UpdateNextId(int maxId)
        {
            nextId = maxId + 1;
        }
        // Metod för att visa inlägg
        public void DisplayPost()
        {
            Console.WriteLine($"Author: {Author}");
            Console.WriteLine($"Post: {Message}");
            Console.WriteLine($"Timestamp: {Timestamp}");
            Console.WriteLine($"Id: {PostId}");
            Console.WriteLine();
        }
    }

    class Guestbook
    {
        private List<GuestbookPost> posts; // Lista med alla gästboksinlägg
                                           // Konstruktor
        public Guestbook()
        {
            posts = new List<GuestbookPost>();
            if (File.Exists("guestbook.json"))
            { //Kontrollera om filen finns
            }
        }
        //Metod för att radera ett inlägg med id
        public void RemovePost(int postId)
        {
            var postToRemove = posts.FirstOrDefault(post => post.PostId == postId);
            //Kontroll om inlägg finns
            if (postToRemove != null)
            {
                posts.Remove(postToRemove);
                Console.WriteLine($"Post with id {postId} deleted.");
            }
            else
            {
                Console.WriteLine($"Post not deleted. Could not find post with id {postId}.");
            }
        }
        // Metod för att invänta knapptryckning på "4"
        private static void WaitForInput(ConsoleKey expectedKey)
        {
            Console.WriteLine("Press 4 to continue to menu...");
            //Loop som känner av inputs
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey();
            } while (key.Key != expectedKey);
            Console.Clear(); //Rensa 
        }

        // Metod för att lägga till inlägg
        public void AddPost()
        { //Visa meddelanden
            Console.WriteLine("CREATE A POST TO THE GUESTBOOK");
            Console.WriteLine("Enter your name:");
            Console.WriteLine();
            string? author = Console.ReadLine();  // Läser in författare från användarinput
            while (string.IsNullOrEmpty(author) || string.IsNullOrWhiteSpace(author)) //Kontrollera så det inte finns tomma fält
            {
                Console.WriteLine("Author must be filled. Press enter to write an author or press 4 to go back to menu"); //Om input saknas, skriv ut 
                var key = Console.ReadKey();
                //Kontroll om 4 trycks
                if (key.Key == ConsoleKey.D4)
                {
                    Console.Clear(); //rensa
                    return;
                }
                // Om knapp tryck, visa igen
                Console.WriteLine("\nEnter your name:");
                author = Console.ReadLine();
            }
            Console.WriteLine("Enter your message:");
            string? message = Console.ReadLine();  // Läser in meddelande från användarinput
            while (string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message)) //Kontroll att de ej finns tomma fält
            {
                Console.WriteLine("Message must be filled. Press enter to write a message or press 4 to go back to menu");
                var key = Console.ReadKey();
                //Kontroll om 4 trycks
                if (key.Key == ConsoleKey.D4)
                {
                    Console.Clear(); //Rensa
                    return; //Om knapptryck, visa igen
                }
                Console.WriteLine("\nEnter your message:");
                message = Console.ReadLine();
            }

            GuestbookPost post = new GuestbookPost(message, author);  // Skapa ett nytt gästboksinlägg
            posts.Add(post);  // Lägger till inlägget i listan
            Console.WriteLine();
            Console.WriteLine("Post added to guestbook");
            Console.WriteLine();
            WaitForInput(ConsoleKey.D4);
        }
        // Metod för att läsa inlägg från en JSON-fil
        public void LoadFromJson(string filePath)
        { //Kontroll om fil finns
            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath); //Läs in från fiö
                    var deserializedPosts = JsonSerializer.Deserialize<List<GuestbookPost>>(json); //deserialisera
                    if (deserializedPosts != null && deserializedPosts.Any())
                    {
                        int maxId = deserializedPosts.Max(post => post.PostId);
                        GuestbookPost.UpdateNextId(maxId);
                    }
                    posts = deserializedPosts ?? new List<GuestbookPost>();
                }
                //Vid fel
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error parsing JSON: {ex.Message}");
                }
            }
        }
        // Metod för att visa alla inlägg i gästboken
        public void DisplayGuestbook()
        {
            Console.Clear(); //Rensa skärmen
            if (posts.Count == 0) //Kontrollera om det finns inlägg i gästboken
            {
                Console.WriteLine("No posts found."); //Om inga inlägg finns, skriv ut 
            } //Finns inlägg
            else
            {   // Loopa igenom varje inlägg och skriv ut
                foreach (var post in posts)
                {
                    post.DisplayPost();
                    Console.WriteLine();
                }
            }
        }
        //Metod för att skriva ut menu och lyssna på användarens val
        public static ConsoleKeyInfo DisplayMenu()
        {
            Console.WriteLine("Welcome to Erikas guestbook!");
            Console.WriteLine("Choose what you want to do.");
            Console.WriteLine("");
            Console.WriteLine("1. Add post to guestbook");
            Console.WriteLine("2. Show all posts in guestbook");
            Console.WriteLine("3. Remove post");
            Console.WriteLine("");
            Console.WriteLine("X. Exit");

            ConsoleKeyInfo key = Console.ReadKey();
            Console.WriteLine();
            return key;
        }
        // Metod för att spara inlägg till en JSON-fil
        public void SaveToJson(string filePath)
        {
            string json = JsonSerializer.Serialize(posts);  // Konverterar till JSON-format
            File.WriteAllText(filePath, json);  // Sparar till filen
        }

        static void Main(string[] args)
        { // ÅÄÖ
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Guestbook guestbook = new Guestbook();
            guestbook.LoadFromJson("guestbook.json"); //Hämta från fil

            bool exit = false;
            do
            {
                ConsoleKeyInfo key = DisplayMenu();
                //Switch-sats för menyval
                switch (key.KeyChar)
                {
                    case '1':
                        Console.Clear();
                        guestbook.AddPost();
                        guestbook.SaveToJson("guestbook.json");
                        break;

                    case '2':
                        Console.Clear();
                        guestbook.DisplayGuestbook();
                        WaitForInput(ConsoleKey.D4);
                        break;

                    case '3':
                        Console.Clear();
                        guestbook.DisplayGuestbook();
                        Console.WriteLine("Enter the ID of the post to remove:");
                        if (int.TryParse(Console.ReadLine(), out int postId))
                        {
                            guestbook.RemovePost(postId);
                            guestbook.SaveToJson("guestbook.json");
                        }
                        WaitForInput(ConsoleKey.D4);
                        break;

                    case 'x':
                        exit = true;
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        WaitForInput(ConsoleKey.D4);
                        break;
                }

            } while (!exit);
        }
    }
}

