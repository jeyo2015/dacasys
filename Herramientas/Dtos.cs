﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Herramientas
{
    public class SessionDto {
        public string Nombre { get; set; }
        public string loginUsuario { get; set; }
        public int IDConsultorio { get; set; }
        public int IDClinica { get; set; }
        public bool IsDacasys { get; set; }
        public int Verificar { get; set; }
        public bool ChangePass { get; set; }
        public int IDRol { get; set; }
    }

    public class UsuarioDto
    {
        public string Nombre { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool? changepass { get; set; }
        public int IDEmpresa { get; set; }
        public int IDRol { get; set; }
        public string ConfirmPass { get; set; }
        public bool Estado { get; set; }
    }

    public class ConsultorioDto
    {
        public int IDConsultorio { get; set; }
        public string Login { get; set; }
        public string NombreClinica { get; set; }
        public string NIT { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public string IDUsuarioCreador { get; set; }
        public bool Estado { get; set; }
        public string Email { get; set; }
        public int? IDIntervalo { get; set; }
        public int IDClinica { get; set; }
        public int State { get; set; }
        public List<TelefonoDto> Telefonos { get; set; }
        public List<TrabajosConsultorioDto> Trabajos { get; set; }

    }

    public class TelefonoDto
    {
        public int ID { get; set; }
        public int IDConsultorio { get; set; }
        public int IDClinica { get; set; }
        public string Telefono { get; set; }
        public string Nombre { get; set; }
        public int State { get; set; }
    }
    public class TrabajosConsultorioDto
    {
        public int ID { get; set; }
        public int IDConsultorio { get; set; }
        public int State { get; set; }
    }
    public class TrabajosClinicaDto
    {
        public int IDClinica { get; set; }
        public int ID { get; set; }
        public List<int> IDConsultorio { get; set; }
        public string Descripcion { get; set; }
        public int State { get; set; }
    }


    public class ClinicaDto
    {
        public int IDClinica { get; set; }
        public string Nombre { get; set; }
        public string Login { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public byte[] logoImagen { get; set; }
        public string Descripcion { get; set; }
        public string Direccion { get; set; }
        public string Nit { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public int CantidadConsultorios { get; set; }
        public List<ConsultorioDto> Consultorios { get; set; }
        public bool Estado { get; set; }
        public List<TrabajosClinicaDto> Trabajos { get; set; }
        public List<TelefonoDto> Telefonos { get; set; }
        public int Status { get; set; }
    }
    public class ModulosTree {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public bool IsChecked { get; set; }
        public List<ModulosTree> Hijos { get; set; }
        public bool IsCollapsed { get; set; }
    }

    public class ListModulesTree{
       public List<ModulosTree> Modules { get; set; }
    }
    
}
