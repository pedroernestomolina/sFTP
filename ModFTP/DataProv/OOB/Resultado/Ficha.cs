using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ModFTP.DataProv.OOB.Resultado
{
    
    public class Ficha 
    {

        public Enumerados.EnumResult Result { get; set; }
        public string Mensaje { get; set; }

        public Ficha ()
        {
            Result = Enumerados.EnumResult.isOk;
            Mensaje = "";
        }

    }

}