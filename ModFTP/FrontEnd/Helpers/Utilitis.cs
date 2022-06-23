using ModFTP.DataProv.OOB.Resultado;
using ModFTP.FrontEnd.Src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


namespace ModFTP.FrontEnd.Helpers
{

    public class Utilitis
    {

        static public Ficha CargarXml()
        {
            var result = new Ficha();

            try
            {
                var doc = new XmlDocument();
                doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"\Conf.XML");

                if (doc.HasChildNodes)
                {
                    foreach (XmlNode nd in doc)
                    {
                        if (nd.LocalName.ToUpper().Trim() == "CONFIGURACION")
                        {
                            foreach (XmlNode nv in nd.ChildNodes)
                            {
                                if (nv.LocalName.ToUpper().Trim() == "MODO")
                                {
                                    if (nv.InnerText.Trim().ToUpper() == "MASTER") 
                                    {
                                        Sistema._isMaster = true;
                                    }
                                }

                                if (nv.LocalName.ToUpper().Trim() == "MASTER")
                                {
                                    foreach (XmlNode mc in nv.ChildNodes) 
                                    {
                                        if (mc.LocalName.ToUpper().Trim() == "RUTAPARAALOJARBOLETIN")
                                        {
                                            Sistema._RutaMaster_ParaAlojarBoletin = @mc.InnerText.Trim();
                                        }
                                        if (mc.LocalName.ToUpper().Trim() == "ENVIARBOLETINA")
                                        {
                                            Sistema._ListaSucursalesEnviarBoletin = mc.InnerText.Trim().Split(',');
                                        }
                                        if (mc.LocalName.ToUpper().Trim() == "AL_SUBIR_BOLETN_ENVIAR_MOVIMIENTOS_INVENTARIO_DESDE_LA_FECHA")
                                        {
                                            Sistema._AL_SUBIR_BOLETN_ENVIAR_MOVIMIENTOS_INVENTARIO_DESDE_LA_FECHA = DateTime.Parse(mc.InnerText);
                                        }
                                    }
                                }


                                if (nv.LocalName.ToUpper().Trim() == "SUCURSAL")
                                {
                                    foreach (XmlNode sc in nv.ChildNodes)
                                    {
                                        if (sc.LocalName.ToUpper().Trim() == "ID")
                                        {
                                            Sistema._IdSucursal = sc.InnerText.Trim().ToUpper();
                                        }
                                        if (sc.LocalName.ToUpper().Trim() == "RUTAUBICACIONBOLETIN")
                                        {
                                            Sistema._RutaUbicacionBoletin = sc.InnerText.Trim();
                                        }
                                        if (sc.LocalName.ToUpper().Trim() == "RUTAUBICACIONCIERRE")
                                        {
                                            Sistema._RutaUbicacionCierre = sc.InnerText.Trim();
                                        }
                                        if (sc.LocalName.ToUpper().Trim() == "EQUIPO")
                                        {
                                            foreach (XmlNode eq in sc.ChildNodes) 
                                            {
                                                if (eq.LocalName.ToUpper().Trim() == "RUTAPARACREARARCHIVOCIERRE")
                                                {
                                                    Sistema._RutaParaCrearArchivoCierre = @eq.InnerText.Trim();
                                                }
                                                if (eq.LocalName.ToUpper().Trim() == "RUTAPARAALOJARCIERRE")
                                                {
                                                    Sistema._RutaParaAlojarCierre = @eq.InnerText.Trim();
                                                }
                                                if (eq.LocalName.ToUpper().Trim() == "RUTAPARAALOJARBOLETIN") 
                                                {
                                                    Sistema._RutaParaAlojarBoletin = @eq.InnerText.Trim();
                                                }
                                                if (eq.LocalName.ToUpper().Trim() == "RUTAPARADESCOMPRIMIRBOLETIN")
                                                {
                                                    Sistema._RutaParaDescomprimirBoletin = @eq.InnerText.Trim();
                                                }
                                            }
                                        }
                                        if (sc.LocalName.ToUpper().Trim() == "ACTUALIZARINVENTARIODEPOSITO")
                                        {
                                            Sistema._ActualizarInventarioDeposito = sc.InnerText.Trim().ToUpper() == "SI" ? true : false;
                                        }
                                        if (sc.LocalName.ToUpper().Trim() == "AL_BAJAR_BOLETN_DEJAR_SOLO_MOVIMIENTOS_KARDEX_DEPOSITO_PRINCIPAL")
                                        {
                                            Sistema._AlBajarBoletinDejarSoloMovimientosKardexDepositoPrincipal  = sc.InnerText.Trim().ToUpper() == "SI" ? true : false;
                                        }
                                    }
                                }


                                if (nv.LocalName.ToUpper().Trim() == "SERVIDORFTP")
                                {
                                    foreach (XmlNode sf in nv.ChildNodes)
                                    {
                                        if (sf.LocalName.ToUpper().Trim() == "HOST")
                                        {
                                            Sistema._FtpHost = @sf.InnerText.Trim();
                                        }
                                        if (sf.LocalName.ToUpper().Trim() == "USUARIO")
                                        {
                                            Sistema._FtpUser = sf.InnerText.Trim();
                                        }
                                        if (sf.LocalName.ToUpper().Trim() == "CLAVE")
                                        {
                                            Sistema._FtpClave= sf.InnerText.Trim();
                                        }
                                    }
                                }

                                if (nv.LocalName.ToUpper().Trim() == "SERVIDOR")
                                {
                                    foreach (XmlNode sv in nv.ChildNodes)
                                    {
                                        if (sv.LocalName.Trim().ToUpper() == "HOST")
                                        {
                                            Sistema._ServidorHost = sv.InnerText.Trim();
                                        }
                                        if (sv.LocalName.Trim().ToUpper() == "BD")
                                        {
                                            Sistema._ServidorBD = sv.InnerText.Trim();
                                        }
                                        if (sv.LocalName.Trim().ToUpper() == "USER")
                                        {
                                            Sistema._ServidorUser = sv.InnerText.Trim();
                                        }
                                    }
                                }
                            
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result.Result = Enumerados.EnumResult.isError;
                result.Mensaje = e.Message;
            }

            return result;
        }

    }

}