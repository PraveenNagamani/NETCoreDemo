using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

[Table("userprofile")] 
public class Profile{
    [NotNull]  
    [Key]
    public required String username{get;set;}
    [NotNull]
    [EmailAddress]
    [Column("email")]
    public required String emailid{get;set;}
    [NotMapped]
    public  IFormFile Photo{get;set;}

}