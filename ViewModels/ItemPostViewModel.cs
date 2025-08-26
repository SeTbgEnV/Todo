using System.ComponentModel.DataAnnotations;

namespace ToDo.Models.ViewModels;

public class ItemPostViewModel
{
    [Required]
    [MaxLength(20)]
    [MinLength(3)]
    public string Task { get; set; } 
}
