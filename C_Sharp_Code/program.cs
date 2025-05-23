using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public enum HistoryAction
{
    Borrow,
    Return
}
public class Book
{
    public int Id { get; }
    public string Title { get; }
    public string Author { get; }
    // public bool IsAvailable { get;  private set; } private set make error at loading
    // using init set only allow to set value at initialization
    // but we need to set value at loading from json file, so we should use [JsonInclude] attribute and add [JsonConstructor] for overloadding
    [JsonInclude]
    public bool IsAvailable { get; private set;}

    public Book(int id, string title, string author)
    {
        Id = id;
        Title = title;
        Author = author;
        IsAvailable = true;
    }
    [JsonConstructor]
    public Book(int id, string title, string author, bool isAvailable)
    {
        Id = id;
        Title = title;
        Author = author;
        IsAvailable = isAvailable;
    }
    public void Borrow()
    {
        if (IsAvailable)
            IsAvailable = false;
    }
    public void Return()
    {
        if (!IsAvailable)
            IsAvailable = true;
    }
}
public class Library
{
    private List<Book> books = new();
    private List<User> users = new();
    private List<HistoryRecord> historyRecords = new();
    private int nextBookId = 1;
    private int nextUserId = 1;
    private const string FilePath = "C:\\Users\\thela\\OneDrive\\Desktop\\library.json";


    // Book operators ================
    public void AddBook(string title, string author, bool available)
    {
        Book new_book = new Book(nextBookId++, title, author);
        // Make sure that available status is truely updated. 
        if (!available) new_book.Borrow();
        books.Add(new_book);
    }
    public List<Book> GetAllBooks() => books;
    public Book? FindBook(int id) => books.FirstOrDefault(b => b.Id == id);

    // User operator ======================
    public void AddUser(string name) => users.Add(new User(name, nextUserId++));
    public IEnumerable<User> GetAlUlUsers() => users;
    public User? FindUser(int id) => users.FirstOrDefault(u => u.Id == id);

    // Borrow and Return with History Logging ==================
    public bool BorrowBook(int id, int userId)
    {
        var book = FindBook(id);
        var user = FindUser(userId);
        if (book == null || !book.IsAvailable)
            return false;
        if(user == null )
            return false;
        book.Borrow();
        // When borrowing a book, we need to log the action
        historyRecords.Add(new HistoryRecord(book.Id, user.Id, HistoryAction.Borrow));  
        return true;

    }
    public bool ReturnBook(int id, int userId)
    {
        var book = FindBook(id);
        var user = FindUser(userId);
        if (book == null || book.IsAvailable || userId == null)
            return false;
        book.Return();
        // When returning a book, we need to log the action
        historyRecords.Add(new HistoryRecord(book.Id, user.Id, HistoryAction.Return));
        return true;
    }

    // History Logging ==================
    public IEnumerable<HistoryRecord> GetHistoryRecords() => historyRecords;


    // File Handling ==========================
    public void SaveToFile()
    {
        var data = new LibraryData
        {
            listBooks = books,
            listUsers = users,
            listHistoryRecords = historyRecords
        };
        var option = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(FilePath, JsonSerializer.Serialize(data, option));
        Console.WriteLine("Saved data to backup file successfully!");
    }

    public void LoadFromFile()
    {
        if (!File.Exists(FilePath))
        {
            Console.WriteLine("The specified file path is not exist!");
            return;
        }
        var jsonData= File.ReadAllText(FilePath);

        if (string.IsNullOrEmpty(jsonData))
        {
            Console.WriteLine("Empty File!");
            return;
        }

        try
        {
            var data = JsonSerializer.Deserialize<LibraryData>(jsonData);

            if (data == null)
            {
                Console.WriteLine("Failed to load data from file.");
                return;
            }
            Console.WriteLine("Loaded data from backup file succesfully!");
            books = data.listBooks ?? new List<Book>();
            users = data.listUsers ?? new List<User>();
            historyRecords = data.listHistoryRecords ?? new List<HistoryRecord>();


            // Restore ID counters to avoid duplicates: 
            nextBookId = books.Any() ? books.Max(b => b.Id) + 1 : 1;
            nextUserId = users.Any() ? users.Max(u => u.Id) + 1 : 1;
        }
        catch
        {
            Console.WriteLine("Failed to load data from file. Invalid JSON format.");
        }

    }
}

public class LibraryDataHandler
{
    private const string FilePath = "C:\\Users\\thela\\OneDrive\\Desktop\\library.json";
    public void SaveData(List<Book> books)
    {
       var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(books, options);
        File.WriteAllText(FilePath, jsonString);
    }
    public List<Book> LoadData()
    {
        if (!File.Exists(FilePath))
            return new List<Book>();
        string jsonString = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<List<Book>>(jsonString) ?? new List<Book>();
    }
}

public class User
{    
    public int Id { get;}
    public string Name { get; } 
    public User (string name, int id)
    {
        Id = id;
        Name = name;
    }
    public override string ToString()
    {
        return $"[{Id}] - {Name}";
    }
}

public class HistoryRecord
{
    public int BookId { get; }
    public int UserId { get; }
    public HistoryAction Action { get; }
    public DateTime Timestamp { get; }

    
    public HistoryRecord(int bookId, int userId, HistoryAction action)
    {
        BookId = bookId;
        Action = action;
        UserId = userId;
        Timestamp = DateTime.Now;
    }

    [JsonConstructor]
    public HistoryRecord(int bookId, int userId, HistoryAction action, DateTime timestamp)
    {
        BookId = bookId;
        Action = action;
        UserId = userId;
        Timestamp = timestamp;
    }

    public override string ToString()
    {
        return $"{Timestamp}: User {UserId}" +
                $"{(Action == HistoryAction.Borrow? " borrowed" :" returned")} Book {BookId}"    ;
    }
}

public class LibraryData
{
    public List<Book> listBooks { get; set; } = new();
    public List<HistoryRecord> listHistoryRecords { get; set; } = new();
    public List<User> listUsers { get; set; } = new();
}

// --- MAIN PROGRAM ---

public static class Program
{
    static Library library = new();
    //static LibraryDataHandler dataHandler = new LibraryDataHandler();


    static void Main()
    {
       // Load all data from backup file: 
        library.LoadFromFile();

        
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Library System");
            Console.WriteLine("1. Add User");
            Console.WriteLine("2. View All Users");
            Console.WriteLine("3. Add Book");   
            Console.WriteLine("4. View All Books");
            Console.WriteLine("5. Borrow Book");
            Console.WriteLine("6. Return Book");
            Console.WriteLine("7. View History Records");
            Console.WriteLine("8. Save");
            Console.WriteLine("9. Exit");
            Console.Write("Select an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    AddUser();
                    break;
                case "2":
                    ShowAllUsers();
                    break;
                case "3":
                    AddBook();
                    break;
                case "4":
                    ViewAllBooks();
                    break;
                case "5":
                    BorrowBook();
                    break;
                case "6":
                    ReturnBook();
                    break;
                case "7":
                    ShowHisoryRecords();
                    break;
                case "8":
                    library.SaveToFile();
                    break;
                case "9":
                    // Do nothing
                    return;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }
    static void AddBook()
    {
        Console.Write("Enter book title: ");
        string ?title = Console.ReadLine();
        if(string.IsNullOrEmpty(title))
        {
            Console.WriteLine("Invalid book title.");
            return;
        }
        Console.Write("Enter book author: ");
        string ?author = Console.ReadLine();
        if(string.IsNullOrEmpty(author))
        {
            Console.WriteLine("Invalid book author.");
            return;
        }
        // Add book to library
        // library.AddBook(title, author, true);
        // Add book to library with available status
        library.AddBook(title, author, true);
        Console.WriteLine("Book added successfully.");
        Console.ReadKey();
    }
    static void ViewAllBooks()
    {
        Console.WriteLine("All Books:");
        foreach (var book in library.GetAllBooks())
        {
            Console.WriteLine($"{book.Id}. {book.Title} by {book.Author} - {(book.IsAvailable ? "Available" : "Not Available")}");
        }
        Console.ReadKey();
    }
    static void BorrowBook()
    {
        Console.Write("Enter user's ID: ");
        if (int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.Write("Enter book ID to borrow: ");
            if (int.TryParse(Console.ReadLine(), out int bookId))
            {
                if (library.BorrowBook(bookId, userId))
                    Console.WriteLine("Book borrowed successfully.");
                else
                    Console.WriteLine("Book not available or does not exist.");
            }
            else
            {
                Console.WriteLine("Invalid Book's ID.");
            }
        }
        else
        {
            Console.WriteLine("Invalid User's ID.");
        }
        Console.ReadKey();
    }
    static void ReturnBook()
    {
        Console.Write("Enter user's ID: ");
        if (int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.Write("Enter book ID to borrow: ");
            if (int.TryParse(Console.ReadLine(), out int bookId))
            {
                if (library.ReturnBook(bookId, userId))
                    Console.WriteLine("Book returned successfully.");
                else
                    Console.WriteLine("Book not available or does not exist.");
            }
            else
            {
                Console.WriteLine("Invalid Book's ID.");
            }
        }
        else
        {
            Console.WriteLine("Invalid User's ID.");
        }
        Console.ReadKey();
    }

    static void ShowHisoryRecords()
    {
        Console.WriteLine("History Records:");
        var records = library.GetHistoryRecords();
        if (!records.Any())
        {
            Console.WriteLine("No history.");
        }
        else
        {
            foreach (var record in records)
            {
                Console.WriteLine(record.ToString());
            }
        }

        Console.ReadKey();
    }
    static void AddUser()
    {
        Console.Write("Enter user name: ");
        string ?name = Console.ReadLine();
        if(string.IsNullOrEmpty(name))
        {
            Console.WriteLine("Invalid user name.");
            return;
        }
        library.AddUser(name);
        Console.WriteLine("User added successfully.");
        Console.ReadKey();
    }
    static void ShowAllUsers()
    {
        Console.WriteLine("All Users:");
        foreach (var user in library.GetAlUlUsers())
        {
            Console.WriteLine(user.ToString());
        }
        Console.ReadKey();
    }
}
