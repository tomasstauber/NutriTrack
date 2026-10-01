using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.MAUI.Models
{
    // IMPORTANTE: los nombres tienen que coincidir EXACTAMENTE con
    // NutriTrack.Core.Entities.Enums.RolUsuario, porque la API los
    // manda como texto ("Administrador", "EncargadoDeCampo"...).
    public enum RolUsuario
    {
        AsesorTecnico,
        EncargadoDeCampo,
        Administrador
    }
}
