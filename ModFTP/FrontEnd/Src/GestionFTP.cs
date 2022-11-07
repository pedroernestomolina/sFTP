using ModFTP.DataProv.OOB.Resultado;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ModFTP.FrontEnd.Src
{

    public class GestionFTP
    {


        //BAJAR BOLETIN
        private string _ruta_UbicacionBoletin;
        //BAJAR CIERRES
        private string _ruta_UbicacionCierresFtp;
        private string _ruta_ParaBajarCierre;

        private string _rutaArchivosTxt_Boletin;

        //HOSTING
        private string _ftpUsername;
        private string _ftpPassword;
        private string _ftpHost;
        //host
        private string _servidor;
        private string _baseDatos;
        private string _usuario;


        private IPosOffLine.IProvider _offLine;
        private bool _master;
        private StringBuilder _st;
        private bool _boletinDescargadoIsOk;
        private bool _prepararCierreIsOk;


        public bool IsMaster { get { return _master;} }
        public string IdSucursal { get { return Sistema._IdSucursal; } }
        public bool DescargaBoletinIsOk { get { return _boletinDescargadoIsOk; } }
        public bool PrepararCierreIsOk { get { return _prepararCierreIsOk; } }
        public string Identifica { get { return Sistema._isMaster ? "MASTER" : "SUCURSAL"; } }


        public GestionFTP()
        {
            _st = new StringBuilder();
            _master = Sistema._isMaster;
            _servidor = Sistema._ServidorHost;
            _baseDatos = Sistema._ServidorBD;
            _usuario = Sistema._ServidorUser;

            _offLine = new ProvPosOffLine.Provider(@"");
            if (_usuario == "")
            {
                _offLine.setServidorRemoto(_servidor, _baseDatos);
            }
            else 
            {
                _offLine.setServidorRemoto(_servidor, _baseDatos, _usuario);
            }

            //SERVIDOR FTP
            _ftpUsername = Sistema._FtpUser;
            _ftpPassword = Sistema._FtpClave;
            _ftpHost = Sistema._FtpHost;

            //_ftpUsername = "leonuxftp@pitabodegas.com";
            //_ftpPassword = "71277128";
            //_ftpHost = "ftp://pitabodegas.com";
            //_ftpUsername = "leonuxftp@grupocostaven.com";
            //_ftpPassword = "71277128lftp";
            //_ftpHost = "ftp://grupocostaven.com";


            //BAJAR BOLETIN
            //_ruta_UbicacionBoletin = "/entrada02/";
            _ruta_UbicacionBoletin = "/" + Sistema._RutaUbicacionBoletin + "/";

            //BAJAR CIERRES
            //_ruta_UbicacionCierresFtp = "/entrada01/";
            _ruta_UbicacionCierresFtp = "/" + Sistema._RutaUbicacionCierre + "/";

            //_ruta_ParaBajarCierre = @"\\192.168.0.185\share2\";
            _ruta_ParaBajarCierre = @"\\" + Sistema._ServidorHost + @"\share2\";

            //RUTA ARCHIVOS TXT SERVIDOR
            //_rutaArchivosTxt_Boletin = @"\\192.168.0.185\share2";
            _rutaArchivosTxt_Boletin = @"\\" + Sistema._ServidorHost + @"\share2";
        }


        Form1 frm;
        public void Inicia() 
        {
            frm = new Form1();
            frm.setControlador(this);
            frm.ShowDialog();
        }


        public Ficha BorrarArchivos(string pathSource, string archivo)
        {
            var rt = new Ficha();

            try
            {
                DirectoryInfo d = new DirectoryInfo(pathSource);
                FileInfo[] Files = d.GetFiles(archivo);
                foreach (FileInfo file in Files)
                {
                    file.Delete();
                }
            }
            catch (Exception ex)
            {
                rt.Mensaje = ex.Message;
                rt.Result = Enumerados.EnumResult.isError;
            }

            return rt;
        }

        public void MsgDebug(string msg)
        {
            _st.AppendLine(msg);
            frm.MuestraMensaje(_st.ToString());
        }

        public bool EnviarBoletin()
        {
            var result = false;
            if (_master)
            {
                return CallEnviarBoletin_New();
            }
            return result;
        }

        //private void CallEnviarBoletin()
        //{
        //    var _tiempoEspera = 10000;
        //    var _rutaArchivoZip_Boletin = Sistema._RutaMaster_ParaAlojarBoletin ;

        //    _st.Clear();
        //    MsgDebug("INICIAR PROCESO: ENVIAR BOLETIN");
        //    MsgDebug("Eliminando Archivos Basura");
        //    MsgDebug("Ruta: " + _rutaArchivosTxt_Boletin);
        //    var rt = BorrarArchivos(_rutaArchivosTxt_Boletin, "*");
        //    if (rt.Result == Enumerados.EnumResult.isError)
        //    {
        //        Helpers.Msg.Error(rt.Mensaje);
        //        return;
        //    }
        //    System.Threading.Thread.Sleep(_tiempoEspera);

        //    MsgDebug("Test BD");
        //    var r01 = _offLine.Servidor_Test();
        //    if (r01.Result == DtoLib.Enumerados.EnumResult.isError)
        //    {
        //        Helpers.Msg.Error(r01.Mensaje);
        //        return;
        //    }
        //    System.Threading.Thread.Sleep(_tiempoEspera);

        //    MsgDebug("GENERANDO CONSULTAS PARA EL BOLETIN");
        //    var r02 = _offLine.Servidor_Principal_CrearBoletin("/var/lib/mysql-files/");
        //    if (r02.Result == DtoLib.Enumerados.EnumResult.isError)
        //    {
        //        Helpers.Msg.Error(r02.Mensaje);
        //        return;
        //    }
        //    MsgDebug("CREANDO PAQUETE BOLETIN");

        //    var origen = _rutaArchivosTxt_Boletin;
        //    var destinoZip = _rutaArchivoZip_Boletin;
        //    var r03 = EmpaquetarBoletin(origen, destinoZip);
        //    if (r03.Result == Enumerados.EnumResult.isError)
        //    {
        //        Helpers.Msg.Error(r03.Mensaje);
        //        return;
        //    }
        //    MsgDebug("VERIFICANDO PAQUETE");

        //    var seguir = false;
        //    var xms = "VERIFIQUE QUE EL ARCHIVO FUE CREADO EXISTOSAMENTE Y QUE SU TAMAÑO SEA MAYOR A 1KB" + Environment.NewLine + r03.MiEntidad;
        //    MessageBox.Show(xms, "*** ALERTA ***", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //    var xm = MessageBox.Show("Continuar Con El Proceso ?", "*** ALERTA ***", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        //    if (xm == System.Windows.Forms.DialogResult.Yes)
        //    {
        //        seguir = true;
        //    }

        //    if (seguir)
        //    {
        //        MsgDebug("SUBIENDO BOLETIN AL FTP");

        //        var r04 = SubirBoletinAlFtp(r03.MiEntidad);
        //        if (r04.Result == Enumerados.EnumResult.isError)
        //        {
        //            Helpers.Msg.Error(r04.Mensaje);
        //            return;
        //        }
        //        MsgDebug("Proceso Realizado Con E X I T O................");

        //        Helpers.Msg.OK("PROCESO FUE REALIZADO CON EXITO..........");
        //    }
        //    else
        //    {
        //        MsgDebug("Proceso Detenido  !!!!!!!!!!!!!!!!!!!!!!");
        //    }
        //}

        public Entidad<string> EmpaquetarBoletin(string dataOrigen, string destino)
        {
            var rt = new Entidad<string>();

            try
            {
                var fecha = DateTime.Now;
                var df = fecha.Year.ToString().Substring(2, 2) + "_";
                df += fecha.Month.ToString().Trim().PadLeft(2, '0') + "_";
                df += fecha.Day.ToString().Trim().PadLeft(2, '0') + "_";
                df += fecha.Hour.ToString().Trim().PadLeft(2, '0') + "_";
                df += fecha.Minute.ToString().Trim().PadLeft(2, '0');
                df += ".zip";

                destino += @"/boletin_" + df;
                ZipFile.CreateFromDirectory(dataOrigen, destino, CompressionLevel.Fastest, true);

                rt.MiEntidad = destino;
            }
            catch (Exception ex)
            {
                rt.MiEntidad = "";
                rt.Mensaje = ex.Message;
                rt.Result = Enumerados.EnumResult.isError;
            }

            return rt;
        }

        //public Ficha SubirBoletinAlFtp(string archivo)
        //{
        //    var rt = new Ficha();

        //    var sc = new List<string>();
        //    for (var x = 0; x <= Sistema._ListaSucursalesEnviarBoletin.Length-1; x++)
        //    {
        //        var s = Sistema._ListaSucursalesEnviarBoletin[x];
        //        if (s.ToString().Trim() != "")
        //        {
        //            var n = "/entrada" + s.ToString().Trim().PadLeft(2,'0') + "/";
        //            sc.Add(n);
        //        }
        //    }

        //    if (sc.Count > 0)
        //    {
        //        try
        //        {
        //            foreach (var f in sc)
        //            {
        //                SubirArchivo(archivo, _ftpHost + f);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            rt.Mensaje = ex.Message;
        //            rt.Result = Enumerados.EnumResult.isError;
        //        }
        //    }
        //    else 
        //    {
        //        MsgDebug("No Se Han Indicado Las Sucursales A Recibir Boletin");
        //    }

        //    return rt;
        //}

        public void SubirArchivo(string archivo, string hosting) 
        {
            var fileName = Path.GetFileName(archivo);
            var request = (FtpWebRequest)WebRequest.Create(hosting + fileName);

            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            using (var fileStream = File.OpenRead(archivo))
            {
                using (var requestStream = request.GetRequestStream())
                {
                    fileStream.CopyTo(requestStream);
                    requestStream.Close();
                }
            }
            var response = (FtpWebResponse)request.GetResponse();
            MsgDebug(response.StatusDescription);
            response.Close();
        }

        public void BajarArchivo(string hosting, string destino)
        {
            FtpWebRequest request =(FtpWebRequest)WebRequest.Create(hosting);
            request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            using (Stream ftpStream = request.GetResponse().GetResponseStream())
            using (Stream fileStream = File.Create(destino))
            {
                ftpStream.CopyTo(fileStream);
            }
        }

        public void BajarBoletin()
        {
            if (!_master)
            {
                CallBajarBoletin_New();
            }
        }

        public void SubirCierreAlFtp()
        {
            if (!_master)
            {
                CallSubirCierreAlFtp();
            }
        }

        private void CallSubirCierreAlFtp()
        {
            var _ruta_Cierres = Sistema._RutaParaAlojarCierre;
            _ruta_Cierres += @"/";
            var _ruta_DataCierres = _ruta_Cierres+@"/Data/";

            _st.Clear();
            MsgDebug("INICIAR PROCESO: SUBIENDO CIERRES");

            try
            {
                DirectoryInfo d = new DirectoryInfo(_ruta_Cierres);
                FileInfo[] Files = d.GetFiles("data*.*");
                foreach (FileInfo file in Files)
                {
                    MsgDebug("Archivo A Subir:" + file.Name);
                    var nombre1 = "/entrada01/" + file.Name;
                    SubirArchivo(file.FullName, _ftpHost + "/entrada01/");
                    MsgDebug("Archivo Alojado En Hosting: " + nombre1);

                    MsgDebug("Archivo Movido A:" + _ruta_DataCierres + file.Name);
                    file.CopyTo(_ruta_DataCierres + file.Name);

                    MsgDebug("Archivo Eliminado De:" + _ruta_Cierres + file.Name);
                    file.Delete();
                }
                MsgDebug("Proceso Realizado Con E X I T O................");
                Helpers.Msg.OK("PROCESO FUE REALIZADO CON EXITO..........");
            }
            catch (Exception ex)
            {
                Helpers.Msg.Error(ex.Message);
            }
        }

        public void BajarCierres()
        {
            if (_master)
            {
                CallBajarCierres();
            }
        }

        private void CallBajarCierres()
        {
            _st.Clear();
            MsgDebug("INICIAR PROCESO: BAJAR CIERRES");

            try
            {
                List<string> files = new List<string>();

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_ftpHost + _ruta_UbicacionCierresFtp);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                MsgDebug("Buscando Cierres");
                while (!reader.EndOfStream)
                {
                    //Application.DoEvents();
                    files.Add(reader.ReadLine());
                }
                reader.Close();
                response.Close();

                var xb = 0;
                foreach (var f in files)
                {
                    if (f.IndexOf("data") > 0)
                    {
                        xb += 1;
                    }
                }

                MsgDebug(xb.ToString() + " Cierres Encontrados");
                if (xb > 0)
                {
                    //DESCARGAR ARCHIVO
                    foreach (var f in files)
                    {
                        if (f.IndexOf("data") > 0)
                        {
                            var ind = f.IndexOf("data");
                            var fn = f.Substring(ind);


                            //BORRAR ARCHIVOS EXISTENTE 
                            var r00 = BorrarArchivos(_ruta_ParaBajarCierre, "*");
                            if (r00.Result == Enumerados.EnumResult.isError)
                            {
                                Helpers.Msg.Error(r00.Mensaje);
                                return;
                            }

                            MsgDebug("Descargando Cierre: " + fn);
                            BajarArchivo(@_ftpHost + @_ruta_UbicacionCierresFtp + @fn, @_ruta_ParaBajarCierre + @fn);

                            var r01 = DescomprimirRegistrarCierre_2(_ruta_ParaBajarCierre, fn);
                            if (r01.Result == Enumerados.EnumResult.isError)
                            {
                                Helpers.Msg.Error(r01.Mensaje);
                                return;
                            }

                            MsgDebug("Eliminando Cierre: " + fn);
                            request = (FtpWebRequest)WebRequest.Create(_ftpHost + _ruta_UbicacionCierresFtp + fn);
                            request.Method = WebRequestMethods.Ftp.DeleteFile;
                            request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);
                            response = (FtpWebResponse)request.GetResponse();
                            response.Close();
                        }
                    }
                }
                MsgDebug("Proceso Realizado Con E X I T O................");
                Helpers.Msg.OK("PROCESO FUE REALIZADO CON EXITO..........");
            }
            catch (Exception ex)
            {
                Helpers.Msg.Error(ex.Message);
            }
        }

        private Ficha DescomprimirRegistrarCierre_2(string ruta, string file)
        {
            var rt = new Ficha();

            try
            {
                MsgDebug("DESCOMPRIMIR ARCHIVO: " + file);
                ZipFile.ExtractToDirectory(ruta + file, ruta);

                MsgDebug("TEST BD");
                var r01 = _offLine.Servidor_Test();
                if (r01.Result == DtoLib.Enumerados.EnumResult.isError)
                {
                    rt.Result = Enumerados.EnumResult.isError;
                    rt.Mensaje = r01.Mensaje;
                    return rt;
                }

                MsgDebug("INSERTAR CIERRE");
                var r02 = _offLine.Servidor_Principal_InsertarCierre("/var/lib/mysql-files/");
                if (r02.Result == DtoLib.Enumerados.EnumResult.isError)
                {
                    rt.Result = Enumerados.EnumResult.isError;
                    rt.Mensaje = r02.Mensaje;
                    return rt;
                }
            }
            catch (Exception ex)
            {
                rt.Mensaje = ex.Message;
                rt.Result = Enumerados.EnumResult.isError;
            }

            return rt;
        }

        public void PrepararCierre()
        {
            _prepararCierreIsOk = false;
            try
            {
                var _ruta_paraCrearArchivoCierre = Sistema._RutaParaCrearArchivoCierre;
                var _ruta_ParaCrearArchivosTempCierre = _ruta_paraCrearArchivoCierre + @"/temp";
                var _ruta_ParaAlojarCierre = Sistema._RutaParaAlojarCierre;


                _st.Clear();
                MsgDebug("INICIAR PROCESO: PREPARAR CIERRE");


                MsgDebug("VERIFICA SI EXISTE UNA JORNADA ABIERTA CON DOCUMENTOS");
                var rt = _offLine.Verificar_ParaPrepararCierre();
                if (rt.Result == DtoLib.Enumerados.EnumResult.isError)
                {
                    Helpers.Msg.Error(rt.Mensaje);
                    return;
                }
                if (rt.Entidad > 0)
                {
                    var msg = @"PROCESO DETENIDO, EXISTE UNA JORNADA ABIERTA CON DOCUMENTOS EN PROCESO" + Environment.NewLine +
                        "SE RECOMIENDA: " + Environment.NewLine +
                        "1. CERRAR LA JORNADA ACTUAL Y PROCEDER A ENVIAR CIERRE " + Environment.NewLine +
                        "2. ESPERAR A REALIZAR EL CIERRE DEL DIA, Y VOLVER A INTENTAR ENVIAR LOS CIERRES";
                    Helpers.Msg.Error(msg);
                    return;
                }


                MsgDebug("Eliminando Archivos");
                var r00 = BorrarArchivos(_ruta_ParaCrearArchivosTempCierre , "*");
                if (r00.Result == Enumerados.EnumResult.isError)
                {
                    Helpers.Msg.Error(r00.Mensaje);
                    return;
                }

                MsgDebug("TEST BD");
                var r01 = _offLine.Servidor_Test();
                if (r01.Result == DtoLib.Enumerados.EnumResult.isError)
                {
                    Helpers.Msg.Error(r01.Mensaje);
                    return ;
                }

                MsgDebug("GENERAR CIERRE");
                var r02 = _offLine.Servidor_Principal_PreprararCierre(Sistema._IdSucursal, _ruta_paraCrearArchivoCierre, _ruta_ParaAlojarCierre);
                if (r02.Result == DtoLib.Enumerados.EnumResult.isError)
                {
                    Helpers.Msg.Error(r01.Mensaje);
                    return;
                }
                MsgDebug("Proceso Realizado Con E X I T O................");
                _prepararCierreIsOk = true;
            }
            catch (Exception ex)
            {
                Helpers.Msg.Error(ex.Message);
            }
        }

        public void InsertarBoletin()
        {
            if (Sistema._isMaster)
            {
                Helpers.Msg.OK("FUNCION APLICA SOLO A SUCURSALES");
                return;
            }

            try
            {
                var ruta_AlojamientoBoletin = Sistema._RutaParaAlojarBoletin;
                var ruta_DescomprimirBoletin = Sistema._RutaParaDescomprimirBoletin;

                _st.Clear();
                MsgDebug("INICIAR PROCESO: INSERTAR/REGISTRAR BOLETIN");

                DirectoryInfo d = new DirectoryInfo(ruta_AlojamientoBoletin);
                FileInfo[] Files = d.GetFiles("boletin*.zip");
                foreach (FileInfo file in Files)
                {

                    MsgDebug("ELIMINAR ARCHIVOS");
                    var rt=BorrarArchivos(ruta_DescomprimirBoletin, "*");
                    if (rt.Result == Enumerados.EnumResult.isError) 
                    {
                        Helpers.Msg.Error(rt.Mensaje);
                        return;
                    }

                    var ruta_copiar= ruta_DescomprimirBoletin+@"/"+file;
                    MsgDebug("COPIAR BOLETIN");
                    file.CopyTo(ruta_copiar , true);

                    MsgDebug("DESCOMPRIMIR ARCHIVO: " + file);
                    ZipFile.ExtractToDirectory(ruta_copiar, ruta_DescomprimirBoletin);

                    MsgDebug("TEST BD");
                    var r01 = _offLine.Servidor_Test();
                    if (r01.Result == DtoLib.Enumerados.EnumResult.isError)
                    {
                        Helpers.Msg.Error(r01.Mensaje);
                        return;
                    }

                    MsgDebug("INSERTAR BOLETIN");
                    var r02 = _offLine.Servidor_Principal_InsertarBoletin(Sistema._IdSucursal, ruta_DescomprimirBoletin);
                    if (r02.Result == DtoLib.Enumerados.EnumResult.isError)
                    {
                        Helpers.Msg.Error(r02.Mensaje);
                        return;
                    }

                    MsgDebug("BOLETIN REGISTRADO");
                }

                if (Sistema._AlBajarBoletinDejarSoloMovimientosKardexDepositoPrincipal)
                {
                    var r1 = _offLine.Sucursal_GetIdDepositoPrincipal_ByCodigoSucursal(Sistema._IdSucursal);
                    if (r1.Result == DtoLib.Enumerados.EnumResult.isError)
                    {
                        Helpers.Msg.Error(r1.Mensaje);
                        return;
                    }
                    if (!String.IsNullOrEmpty(r1.Entidad))
                    {
                        var r2 = _offLine.Servidor_Principal_EliminarMovimientosKardexExcluyeDeposito(r1.Entidad);
                        if (r2.Result == DtoLib.Enumerados.EnumResult.isError)
                        {
                            Helpers.Msg.Error(r2.Mensaje);
                            return;
                        }
                        MsgDebug("MOVIMIENTOS KARDEX ELIMINADOS");
                    }
                }

                if (Sistema._ActualizarInventarioDeposito) 
                {
                    var r03 = _offLine.Servidor_Principal_ActualizarInventarioDeposito();
                    if (r03.Result == DtoLib.Enumerados.EnumResult.isError)
                    {
                        Helpers.Msg.Error(r03.Mensaje);
                        return;
                    }

                    MsgDebug("DEPOSITO ACTUALIZADO");
                }

                MsgDebug("Proceso Realizado Con E X I T O................");
                Helpers.Msg.OK("PROCESO DE INSERTADO FUE REALIZADO CON EXITO..........");
            }
            catch (Exception ex)
            {
                Helpers.Msg.Error(ex.Message);
            }
        }

        private Ficha InsertarCierreManual()
        {
            var rt = new Ficha();

            try
            {
                _st.Clear();

                MsgDebug("TEST BD");
                var r01 = _offLine.Servidor_Test();
                if (r01.Result == DtoLib.Enumerados.EnumResult.isError)
                {
                    rt.Result = Enumerados.EnumResult.isError;
                    rt.Mensaje = r01.Mensaje;
                    return rt;
                }

                MsgDebug("INSERTAR CIERRE");
                var r02 = _offLine.Servidor_Principal_InsertarCierre("/var/lib/mysql-files/");
                if (r02.Result == DtoLib.Enumerados.EnumResult.isError)
                {
                    rt.Result = Enumerados.EnumResult.isError;
                    rt.Mensaje = r02.Mensaje;
                    return rt;
                }
            }
            catch (Exception ex)
            {
                rt.Mensaje = ex.Message;
                rt.Result = Enumerados.EnumResult.isError;
            }

            return rt;
        }

        public void Proceso_InsertarCierreManual()
        {
            var r01 = InsertarCierreManual();
            if (r01.Result == Enumerados.EnumResult.isError)
            {
                Helpers.Msg.Error(r01.Mensaje);
                return;
            }

            MsgDebug("Proceso Realizado Con E X I T O................");
            Helpers.Msg.OK("PROCESO DE INSERTADO FUE REALIZADO CON EXITO..........");
        }


        private void CallBajarBoletin_New()
        {
            _boletinDescargadoIsOk = false;
            var _ruta_ParaBajarBoletin = Sistema._RutaParaAlojarBoletin;
            _ruta_ParaBajarBoletin += @"/";

            _st.Clear();
            MsgDebug("INICIAR PROCESO: BAJAR BOLETIN");

            try
            {
                MsgDebug("Eliminando Boletines Viejos");
                var r01 = BorrarArchivos(_ruta_ParaBajarBoletin, "boletin*.*");
                if (r01.Result == Enumerados.EnumResult.isError)
                {
                    Helpers.Msg.Error(r01.Mensaje);
                    return;
                }
                var r02 = _offLine.MonitorBoletin_Info();
                if (r02.Result == DtoLib.Enumerados.EnumResult.isError) 
                {
                    Helpers.Msg.Error(r02.Mensaje);
                    return;
                }

                _ruta_UbicacionBoletin = @"//entradaBol//";
                string contents = r02.Entidad;
                if (contents!=null)
                {
                    MsgDebug("Descargando Boletin: " + contents);
                    BajarArchivo(@_ftpHost + @_ruta_UbicacionBoletin + contents, @_ruta_ParaBajarBoletin + contents);
                }
                MsgDebug("Proceso Realizado Con E X I T O................");
                Helpers.Msg.OK("PROCESO DE DESCARGA FUE REALIZADO CON EXITO..........");
                _boletinDescargadoIsOk = true;
            }
            catch (Exception ex)
            {
                MsgDebug("PROCESO FINALIZO CON ERROR...");
                Helpers.Msg.Error(ex.Message);
            }
        }

        private bool CallEnviarBoletin_New()
        {
            var result = false;
            var _tiempoEspera = 10000;
            var _rutaArchivoZip_Boletin = Sistema._RutaMaster_ParaAlojarBoletin;

            _st.Clear();
            MsgDebug("INICIAR PROCESO: ENVIAR BOLETIN");
            MsgDebug("Eliminando Archivos Basura");
            MsgDebug("Ruta: " + _rutaArchivosTxt_Boletin);
            var rt = BorrarArchivos(_rutaArchivosTxt_Boletin, "*");
            if (rt.Result == Enumerados.EnumResult.isError)
            {
                Helpers.Msg.Error(rt.Mensaje);
                return false;
            }
            System.Threading.Thread.Sleep(_tiempoEspera);

            MsgDebug("Test BD");
            var r01 = _offLine.Servidor_Test();
            if (r01.Result == DtoLib.Enumerados.EnumResult.isError)
            {
                Helpers.Msg.Error(r01.Mensaje);
                return false;
            }
            System.Threading.Thread.Sleep(_tiempoEspera);

            MsgDebug("GENERANDO CONSULTAS PARA EL BOLETIN");
            var _fechaMovInv = Sistema._AL_SUBIR_BOLETN_ENVIAR_MOVIMIENTOS_INVENTARIO_DESDE_LA_FECHA;
            var r02 = _offLine.Servidor_Principal_CrearBoletin("/var/lib/mysql-files/", _fechaMovInv);
            if (r02.Result == DtoLib.Enumerados.EnumResult.isError)
            {
                Helpers.Msg.Error(r02.Mensaje);
                return false;
            }
            MsgDebug("CREANDO PAQUETE BOLETIN");

            var origen = _rutaArchivosTxt_Boletin;
            var destinoZip = _rutaArchivoZip_Boletin;
            var r03 = EmpaquetarBoletin(origen, destinoZip);
            if (r03.Result == Enumerados.EnumResult.isError)
            {
                Helpers.Msg.Error(r03.Mensaje);
                return false;
            }
            MsgDebug("VERIFICANDO PAQUETE");

            var seguir = false;
            var xms = "VERIFIQUE QUE EL ARCHIVO FUE CREADO EXITOSAMENTE Y QUE SU TAMAÑO SEA MAYOR A 1KB" + Environment.NewLine + r03.MiEntidad;
            MessageBox.Show(xms, "*** ALERTA ***", MessageBoxButtons.OK, MessageBoxIcon.Information);
            var xm = MessageBox.Show("Continuar Con El Proceso ?", "*** ALERTA ***", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (xm == System.Windows.Forms.DialogResult.Yes)
            {
                seguir = true;
            }

            if (seguir)
            {
                MsgDebug("SUBIENDO BOLETIN AL FTP");

                var r04 = SubirBoletinAlFtp_New(r03.MiEntidad);
                if (r04.Result == Enumerados.EnumResult.isError)
                {
                    Helpers.Msg.Error(r04.Mensaje);
                    return false;
                }
                MsgDebug("Proceso Realizado Con E X I T O................");

                //Helpers.Msg.OK("PROCESO FUE REALIZADO CON EXITO..........");
                result = true;
            }
            else
            {
                MsgDebug("Proceso Detenido  !!!!!!!!!!!!!!!!!!!!!!");
            }

            return result;
        }

        public Ficha SubirBoletinAlFtp_New(string archivo)
        {
            var rt = new Ficha();

            try
            {
                var ind = archivo.IndexOf("boletin");
                var contenido = "";
                if (ind > 0)
                {
                    contenido = archivo.Substring(ind);
                }
                var ubicacion = archivo.Substring(0, ind);

                MsgDebug("ACTUALIZANDO MONITOR BOLETN DEL HOSTING");
                var r01 = _offLine.MonitorBoletin_Actualizar(contenido);
                if (r01.Result == DtoLib.Enumerados.EnumResult.isError)
                {
                    MsgDebug("PROCESO FINALIZO CON ERROR");
                    rt.Mensaje = r01.Mensaje;
                    rt.Result = Enumerados.EnumResult.isError;
                    return rt;
                }
                MsgDebug("Subiendo Boletin: " + archivo);
                SubirArchivo(archivo, _ftpHost + @"//entradaBol//");
                MsgDebug("PROCESO REALIZADO CON EXITO.......");
            }
            catch (Exception ex)
            {
                MsgDebug("PROCESO FINALIZO CON ERROR.......");
                rt.Mensaje = ex.Message;
                rt.Result = Enumerados.EnumResult.isError;
            }

            return rt;
        }


        public Ficha BuscarCambiosBD()
        {
            var rt = new Ficha();

            var r01 = _offLine.MonitorCambiosBD_GetId_UltimoCambioRegistrado();
            if (r01.Result == DtoLib.Enumerados.EnumResult.isError)
            {
                MsgDebug("PROCESO FINALIZO CON ERROR");
                rt.Mensaje = r01.Mensaje;
                rt.Result = Enumerados.EnumResult.isError;
                return rt;
            }
            MsgDebug("BUSCANDO ID ULTIMO CAMBIO PROCESADO: "+r01.Entidad.ToString());

            var r02 = _offLine.MonitorCambiosBD_Host_GetLista_NuevosCambios_APartirDel_IdRef(r01.Entidad);
            if (r02.Result == DtoLib.Enumerados.EnumResult.isError)
            {
                MsgDebug("PROCESO FINALIZO CON ERROR");
                rt.Mensaje = r02.Mensaje;
                rt.Result = Enumerados.EnumResult.isError;
                return rt;
            }
            MsgDebug("BUSCANDO LISTA DE CAMBIOS NUEVOS A PROCESAR: " + r02.Lista.Count.ToString());

            if (r02.Lista.Count > 0)
            {
                List<DtoLibPosOffLine.Servidor.MonitorCambiosBD.NevoCambio.Ficha> _lCmb= new List<DtoLibPosOffLine.Servidor.MonitorCambiosBD.NevoCambio.Ficha>();
                switch (Sistema.TipoSistemaFact)
                {
                    case EnumeradoSist.TipoSistema.PosOffLine:
                        _lCmb = r02.Lista.Where(w => w.aplicaPosOffLine.Trim().ToUpper() == "1").ToList();
                        break;
                    case EnumeradoSist.TipoSistema.PosOnLine:
                        _lCmb = r02.Lista.Where(w => w.aplicaPosOnLine.Trim().ToUpper() == "1").ToList();
                        break;
                };
                var r03 = _offLine.MonitorCambiosBD_ProcesarCambios(_lCmb);
                if (r03.Result == DtoLib.Enumerados.EnumResult.isError)
                {
                    MsgDebug("PROCESO FINALIZO CON ERROR");
                    rt.Mensaje = r03.Mensaje;
                    rt.Result = Enumerados.EnumResult.isError;
                    return rt;
                }
                MsgDebug("EJCUTANDO CAMBIOS");
            }
            else 
            {
                MsgDebug("NO HAY CAMBIOS QUE PROCESAR");
            }
            return rt;
        }

        public void DescargarActualizacionBD()
        {
            if (Sistema._isMaster)
            {
                Helpers.Msg.OK("FUNCION APLICA SOLO A SUCURSALES");
                return;
            }
            var r01 = BuscarCambiosBD();
            if (r01.Result == Enumerados.EnumResult.isError) 
            {
                Helpers.Msg.Error(r01.Mensaje);
                return;
            }
            Helpers.Msg.OK("ACTUALIZACIONES REALIZADAS CON EXITO");
        }

        public void GestionMaster()
        {
            if (EnviarBoletin())
            {
                BajarCierres();
            }
        }

    }

}