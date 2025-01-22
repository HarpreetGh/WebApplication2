
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public class User {
    public required string Name {get; set;}
    public required string Email {get; set;}
    public required string Password {get; set;}

    private string hash {get; set;}

    public string SerializeUserData() {
        if (ValidUserData()) {
            EncryptData();
            hash = GenerateHash();
            return JsonSerializer.Serialize(this);
        }
        else {
            Console.WriteLine("User Data is not valid");
            return string.Empty;
        }
    }

    public bool ValidUserData() {
        // This sends data only if it's valid
        return !(string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password));
    }

    private void EncryptData() {
        // This protects Password even when sent.
        Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(Password));
        Console.WriteLine("Encrypted");
    }

    public User? DeserializeUserData(string sUser, bool isTrustedSource) {
        // This protects system from Deserializing from unknown source.
        if (isTrustedSource) {
            string inHash = GenerateHash(sUser);
            if (string.Equals(hash, inHash)) {
                return JsonSerializer.Deserialize<User>(sUser);
            }
            else {
                Console.WriteLine("Hash not matched");
                return null;
            }
        }
        else { 
            Console.WriteLine("Not Trusted Source");
            return null;
        }
    }

    private string GenerateHash(string? u = null) {
        using(SHA256 sha = SHA256.Create()) {
            byte[] hashBits = sha.ComputeHash(Encoding.UTF8.GetBytes(u ?? JsonSerializer.Serialize(this)));
            return Convert.ToBase64String(hashBits);
        }
    }
}

public class Program {
    public static void Main() {
        User u = new User { Name = "Tony", Email = "T@gmail.com", Password = "P@ssword123"};
        string ds = u.SerializeUserData();

        Console.WriteLine($"ds {ds}");

        // var deSel = u.DeserializeUserData("{\"Name\": \"Tim\", \"Email\": \"Tim@gmail.com\", \"Password\": \"Te$t\"}", true);
        var deSel = u.DeserializeUserData(ds, true);
        Console.WriteLine($"deSel?.Name {deSel?.Name}");

    }
}