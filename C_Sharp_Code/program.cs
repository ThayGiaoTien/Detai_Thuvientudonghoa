using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

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
    private int nextId = 1;

    public void AddBook(string title, string author, bool available)
    {
        Book new_book = new Book(nextId++, title, author);
        // Make sure that available status is truely updated. 
        if (!available) new_book.Borrow();
        books.Add(new_book);
    }
    public List<Book> GetAllBooks() => books;
    public Book? FindBook(int id) => books.FirstOrDefault(b => b.Id == id);
    public bool BorrowBook(int id)
    {
        var book = FindBook(id);
        if (book == null || !book.IsAvailable)
            return false;
        book.Borrow();
        return true;

    }

    public bool ReturnBook(int id)
    {
        var book = FindBook(id);
        if (book == null || book.IsAvailable)
            return false;
        book.Return();
        return true;    
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
public static class Program
{
    static Library library = new();
    static LibraryDataHandler dataHandler = new LibraryDataHandler();


    static void Main()
    {
        var books = dataHandler.LoadData();
        foreach (var book in books)
        {
            library.AddBook(book.Title, book.Author, book.IsAvailable);
        }
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Library System");
            Console.WriteLine("1. Add Book");   
            Console.WriteLine("2. View All Books");
            Console.WriteLine("3. Borrow Book");
            Console.WriteLine("4. Return Book");    
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    AddBook();
                    break;
                case "2":
                    ViewAllBooks();
                    break;
                case "3":
                    BorrowBook();
                    break;
                case "4":
                    ReturnBook();
                    break;
                case "5":
                    dataHandler.SaveData(library.GetAllBooks());
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
        string title = Console.ReadLine();
        Console.Write("Enter book author: ");
        string author = Console.ReadLine();
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
        Console.Write("Enter book ID to borrow: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            if (library.BorrowBook(id))
                Console.WriteLine("Book borrowed successfully.");
            else
                Console.WriteLine("Book not available or does not exist.");
        }
        else
        {
            Console.WriteLine("Invalid ID.");
        }
        Console.ReadKey();
    }
    static void ReturnBook()
    {
        Console.Write("Enter book ID to return: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            if (library.ReturnBook(id))
                Console.WriteLine("Book returned successfully.");
            else
                Console.WriteLine("Book not borrowed or does not exist.");
        }
        else
        {
            Console.WriteLine("Invalid ID.");
        }
        Console.ReadKey();
    }


}using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

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
    private int nextId = 1;

    public void AddBook(string title, string author, bool available)
    {
        Book new_book = new Book(nextId++, title, author);
        // Make sure that available status is truely updated. 
        if (!available) new_book.Borrow();
        books.Add(new_book);
    }
    public List<Book> GetAllBooks() => books;
    public Book? FindBook(int id) => books.FirstOrDefault(b => b.Id == id);
    public bool BorrowBook(int id)
    {
        var book = FindBook(id);
        if (book == null || !book.IsAvailable)
            return false;
        book.Borrow();
        return true;

    }

    public bool ReturnBook(int id)
    {
        var book = FindBook(id);
        if (book == null || book.IsAvailable)
            return false;
        book.Return();
        return true;    
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
public static class Program
{
    static Library library = new();
    static LibraryDataHandler dataHandler = new LibraryDataHandler();


    static void Main()
    {
        var books = dataHandler.LoadData();
        foreach (var book in books)
        {
            library.AddBook(book.Title, book.Author, book.IsAvailable);
        }
        while (true)
        {
            Console.Clear();
            Console.WriteLine("Library System");
            Console.WriteLine("1. Add Book");   
            Console.WriteLine("2. View All Books");
            Console.WriteLine("3. Borrow Book");
            Console.WriteLine("4. Return Book");    
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");

            switch (Console.ReadLine())
            {
                case "1":
                    AddBook();
                    break;
                case "2":
                    ViewAllBooks();
                    break;
                case "3":
                    BorrowBook();
                    break;
                case "4":
                    ReturnBook();
                    break;
                case "5":
                    dataHandler.SaveData(library.GetAllBooks());
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
        string title = Console.ReadLine();
        Console.Write("Enter book author: ");
        string author = Console.ReadLine();
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
        Console.Write("Enter book ID to borrow: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            if (library.BorrowBook(id))
                Console.WriteLine("Book borrowed successfully.");
            else
                Console.WriteLine("Book not available or does not exist.");
        }
        else
        {
            Console.WriteLine("Invalid ID.");
        }
        Console.ReadKey();
    }
    static void ReturnBook()
    {
        Console.Write("Enter book ID to return: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            if (library.ReturnBook(id))
                Console.WriteLine("Book returned successfully.");
            else
                Console.WriteLine("Book not borrowed or does not exist.");
        }
        else
        {
            Console.WriteLine("Invalid ID.");
        }
        Console.ReadKey();
    }


}
