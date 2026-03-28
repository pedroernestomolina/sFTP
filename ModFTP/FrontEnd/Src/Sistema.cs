using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ModFTP.FrontEnd.Src
{

    public class enumNegocio 
    {
        public enum Negocio { NoDefinido=-1, MayoristaValencia = 1, PitaGuacara = 2 };
    }
    public class Sistema
    {

        //IDENTIFICA EL NEGOCIO A CONTROLAR BOLETIN/CIERRE
        public static enumNegocio.Negocio IdNegocio { get; set; }
        public static string CarpetaBoletin_Negocio { get; set; }
        public static string CarpetaCierre_Negocio { get; set; }


        public static bool _isMaster { get; set; }

        public static string _ServidorHost { get; set; }
        public static string _ServidorBD { get; set; }
        public static string _ServidorUser { get; set; }

        public static string _FtpHost { get; set; }
        public static string _FtpUser { get; set; }
        public static string _FtpClave { get; set; }

        public static string _IdSucursal { get; set; }
        public static string _RutaUbicacionBoletin { get; set; }
        public static string _RutaUbicacionCierre { get; set; }

        public static string _RutaMaster_ParaAlojarBoletin { get; set; }

        public static string _RutaParaCrearArchivoCierre  {get;set;}
        public static string _RutaParaAlojarCierre  {get;set;}
        public static string _RutaParaAlojarBoletin {get;set;}
        public static string _RutaParaDescomprimirBoletin {get;set;}

        public static string[] _ListaSucursalesEnviarBoletin { get; set; }
        public static bool _ActualizarInventarioDeposito { get; set; }
        public static bool _AlBajarBoletinDejarSoloMovimientosKardexDepositoPrincipal { get; set; }

        public static DateTime _AL_SUBIR_BOLETN_ENVIAR_MOVIMIENTOS_INVENTARIO_DESDE_LA_FECHA { get; set; } // FORMATO AÑO,MES,DIA

        public static EnumeradoSist.TipoSistema TipoSistemaFact { get; set; }
    }
}