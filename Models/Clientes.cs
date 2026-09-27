namespace ApiEmpresa.Models;

using System .ComponentModel.DataAnnotations;
public class Clientes
{
    [Key]
    public Int32 Id_Cliente { get; set; }

    public String? cui { get; set; }
    public String? nit { get; set; }
    public String? nombres { get; set; }
    public String? apellidos { get; set; }
    public String? direccion { get; set; }
    public String? telefono { get; set; }
    public DateTime? fecha_nacimiento { get; set; }
    
}
