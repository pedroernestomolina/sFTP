﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ModFTP.FrontEnd.Src
{

    public partial class Form1 : Form
    {


        private GestionFTP _controlador;


        public Form1()
        {
            InitializeComponent();
        }


        private void BT_BAJAR_BOLETIN_Click(object sender, EventArgs e)
        {
            BajarBoletin();
            InsertarBoletin();
        }

        private void BajarBoletin()
        {
            _controlador.BajarBoletin();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            L_DEBUG.Text = "";
            BT_ENVIAR_BOLETIN.Enabled = _controlador.IsMaster;
            //BT_BAJAR_CIERRE.Enabled = _controlador.IsMaster;

            BT_BAJAR_BOLETIN.Enabled = !_controlador.IsMaster;
            BT_SUBIR_CIERRE.Enabled = !_controlador.IsMaster;
           // BT_PREPARAR_CIERRE.Enabled = !_controlador.IsMaster;
           // BT_INSERTAR_BOLETIN.Enabled = !_controlador.IsMaster;

            panel15.Visible = !_controlador.IsMaster;
            L_VERSION.Text = "Ver. " + Application.ProductVersion;
            L_SUCURSAL.Text = _controlador.IdSucursal;
            L_ID.Text = _controlador.Identifica;
        }

        private void BT_SUBIR_CIERRE_Click(object sender, EventArgs e)
        {
            SubirCierreAlFtp();
        }

        private void SubirCierreAlFtp()
        {
            var r01 = _controlador.BuscarCambiosBD();
            if (r01.Result == DataProv.OOB.Resultado.Enumerados.EnumResult.isError)
            {
                Helpers.Msg.Error(r01.Mensaje);
                return;
            }
            _controlador.PrepararCierre();
            if (_controlador.PrepararCierreIsOk)
            {
                _controlador.SubirCierreAlFtp();
            }
        }

        private void BT_ENVIAR_BOLETIN_Click(object sender, EventArgs e)
        {
            EnviarBoletin();
        }
        private void EnviarBoletin()
        {
            _controlador.GestionMaster();
            //_controlador.EnviarBoletin();
        }

        private void BT_BAJAR_CIERRE_Click(object sender, EventArgs e)
        {
            BajarCierres();
        }
        private void BajarCierres()
        {
            _controlador.BajarCierres();
        }

        public void setControlador(GestionFTP ctr)
        {
            _controlador = ctr;
        }

        public void MuestraMensaje(string msg)
        {
            L_DEBUG.Text = msg ;
            this.Refresh();
        }

        private void BT_PREPARAR_CIERRE_Click(object sender, EventArgs e)
        {
            PrepararCierre();
        }
        private void PrepararCierre()
        {
            _controlador.PrepararCierre();
        }

        private void BT_INSERTAR_BOLETIN_Click(object sender, EventArgs e)
        {
            InsertarBoletin();
        }
        private void InsertarBoletin()
        {
            if (_controlador.DescargaBoletinIsOk)
                _controlador.InsertarBoletin();
        }

        private void TSM_SISTEMA_INSERTAR_BOLETIN_MANUAL_Click(object sender, EventArgs e)
        {
            InsertarBoletinManual();
        }
        private void InsertarBoletinManual()
        {
            _controlador.InsertarBoletin();
        }

        private void TSM_SISTEMA_INSERTAR_CIERRE_MANUAL_Click(object sender, EventArgs e)
        {
            InsertarCierreManual();
        }
        private void InsertarCierreManual()
        {
            _controlador.Proceso_InsertarCierreManual();
        }

        private void TSM_ARCHIVO_SALIR_Click(object sender, EventArgs e)
        {
            Salida();
        }
        private void BT_SALIR_Click(object sender, EventArgs e)
        {
            Salida();
        }
        private void Salida()
        {
            this.Close();
        }

        private void TDM_SISTEMA_DESCARGAR_ACTUALIZACIONES_BD_Click(object sender, EventArgs e)
        {
            DescargarActualizacionBD();
        }
        private void DescargarActualizacionBD()
        {
            _controlador.DescargarActualizacionBD();
        }


        private void TSM_Sistema_SoloSubirBoletin_Click(object sender, EventArgs e)
        {
            _controlador.EnviarBoletin();
        }
        private void TSM_Sistema_SoloBajarCierres_Click(object sender, EventArgs e)
        {
            _controlador.BajarCierres();
        }
     
    }

}