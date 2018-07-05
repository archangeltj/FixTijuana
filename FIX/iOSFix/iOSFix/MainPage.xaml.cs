using System;
using Xamarin.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace iOSFix
{
    public partial class MainPage : ContentPage
    {
        private int iVehiculo;
        private int IDInventario;
        int _Ciudad = 1;
        //DESARROLLO
        private string ConnStr = "Password=Bl00d231295;Persist Security Info=False;User ID=porsh_wflow;Initial Catalog=porshe17_testfix;Data Source=192.185.11.48";
        private string ConnStr2 = "Password=Bl00d231295;Persist Security Info=False; User ID=porsh_wflow;Initial Catalog=porshe17_testfix;Data Source=192.185.11.48";
        //private string ConnStr = "Password=Bl00d231295;Persist Security Info=False;" +
        //    "User ID=porsh_wflow;Initial Catalog=porshe17_Workflow;Data Source=192.185.11.48";
        //private string ConnStr2 = "Password=Bl00d231295;Persist Security Info=False;" +
        //    "User ID=porsh_wflow;Initial Catalog=porshe17_wflowmxli;Data Source=192.185.11.48";
        //PRODUCCION
        //private string ConnStr = "Password=Bl00d231295;Persist Security Info=False;" +
        //    "User ID=porsh_wflow;Initial Catalog=porshe17_Workflow;Data Source=127.0.0.1";
        //private string ConnStr2 = "Password=Bl00d231295;Persist Security Info=False;" +
        //    "User ID=porsh_wflow;Initial Catalog=porshe17_wflowmxli;Data Source=127.0.0.1";

        //TEST

        //private string ConnStr = "Password=Bl00d231295;Persist Security Info=False;" +
        //    "User ID=porsh_wflow;Initial Catalog=porshe17_testfix;Data Source=127.0.0.1";
        //private string ConnStr2 = "Password=Bl00d231295;Persist Security Info=False;" +
        //    "User ID=porsh_wflow;Initial Catalog=porshe17_testfix;Data Source=127.0.0.1";

        public MainPage()
        {
            InitializeComponent();
            _Ciudad = 1; //poner en 2 si es mexicali
            CargaUsuarios();
        }
        private class Usuarios
        {
            public int ID { get; set; }
            public string Nombre { get; set; }
        }
        private void CargaUsuarios()
        {
            DataSet d = Ejecuta("select iduser, name from porsh_admin.cat_users where status=1 and usertype=2 order by name");
            List<Usuarios> Us = new List<Usuarios>();
            foreach (DataRow r in d.Tables[0].Rows)
            {
                Usuarios x = new Usuarios();
                x.ID = int.Parse(r["iduser"].ToString());
                x.Nombre = r["name"].ToString();
                Us.Add(x);
            }
            cmbUsuario.ItemDisplayBinding = new Binding( "Nombre");
            cmbUsuario.ItemsSource = Us;
        }


        public void BuscaDatos()
        {
            try
            {
                IDInventario = 0;
                string s = "select Brand +', ' + Model + ', ' + cast(Year as varchar) + ', ' + Color as Vehiculo from porsh_admin.tbl_vehicles where IDVehicles=" + txtID.Text;
                DataSet d =  Ejecuta(s);
                if (d.Tables[0].Rows.Count > 0)
                {
                    DataRow r = d.Tables[0].Rows[0];
                    lblDatosVehculo.Text = r["Vehiculo"].ToString();
                    btnSave.IsEnabled = true;
                    iVehiculo = int.Parse(txtID.Text);
                    d = InventarioSel();
                    if (d.Tables[0].Rows.Count > 0)
                        LoadInventario();
                }
                else {
                    DisplayAlert("FIX", "No se encontro informacion del Vehiculo", "OK");
                    btnSave.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("FIX", ex.Message, "OK");
            }
        }
        private DataSet InventarioSel()
        {
            string s = "select * from porsh_admin.tbl_InventarioVehiculo where idVehiculo=" + iVehiculo.ToString();
            return Ejecuta(s);
        }
        public DataSet InventSelDelanteroFrontal(int IDInventario)
        {
            string s = "select * from porsh_admin.det_DelanteroFrontal where IDInventario=" + IDInventario.ToString();
            return Ejecuta(s);
        }
        public DataSet InventSelDerecho(int IDInventario)
        {
            string s = "select * from porsh_admin.det_CostadoDerecho where IDInventario=" + IDInventario.ToString();
            return Ejecuta(s);
        }
        public DataSet InventSelIzquierdo(int IDInventario)
        {
            string s = "select * from porsh_admin.det_CostadoIzquierdo where IDInventario=" + IDInventario.ToString();
            return Ejecuta(s);
        }
        public DataSet InventSelExtTrasera(int IDInventario)
        {
            string s = "select * from porsh_admin.det_ExtTrasera where IDInventario=" + IDInventario.ToString();
            return Ejecuta(s);
        }
        public DataSet InventSelInterior(int IDInventario)
        {
            string s = "select * from porsh_admin.det_Interior where IDInventario=" + IDInventario.ToString();
            return Ejecuta(s);
        }
        public DataSet InventSelMotor(int IDInventario)
        {
            string s = "select * from porsh_admin.det_motor where IDInventario=" + IDInventario.ToString();
            return Ejecuta(s);
        }
        private void LoadInventario()
        {
            try
            {
                DataSet d = InventarioSel();
                if (d.Tables[0].Rows.Count > 0)
                {
                    DataRow r = d.Tables[0].Rows[0];
                    IDInventario = int.Parse(r[0].ToString());
                    cmbTrans.SelectedIndex = 0;
                    if ((bool)r["Automatico"])
                        cmbTrans.SelectedIndex= 1;
                    switch (r["Cilindros"].ToString())
                    {
                        case "4": cmbCilindros.SelectedIndex = 0; break;
                        case "6": cmbCilindros.SelectedIndex = 1; break;
                        case "8": cmbCilindros.SelectedIndex = 2; break;
                    }
                    txtKMIni.Text = r["kminicial"].ToString();
                    txtKMFin.Text = r["kmfin"].ToString();
                    txtComentarios.Text = r["Comentarios"].ToString();
                    txtCondLlantas.Text = r["condicionesllantas"].ToString();
                    txtMarcaLlantas.Text = r["marcallantas"].ToString();
                    txtMedidaLLantas.Text = r["medidallantas"].ToString();
                    txtDamPree.Text = r["preexistente"].ToString();
                    IDInventario = int.Parse(d.Tables[0].Rows[0][0].ToString());
                    //buscamos exterior delantera frontal
                    r = InventSelDelanteroFrontal(IDInventario).Tables[0].Rows[0];
                    chkParabrisas.IsToggled = (bool)r["Parabrisas"];
                    chkParrilla.IsToggled = (bool)r["Parrilla"];
                    chkPortaPlaca.IsToggled = (bool)r["Portaplaca"];
                    chkBLimp.IsToggled = (bool)r["Limpiadores"];
                    chkHulesLimp.IsToggled = (bool)r["HulesLimpiadores"];
                    chkBiceles.IsToggled = (bool)r["Biceles"];
                    chkAntena.IsToggled = (bool)r["Antena"];
                    chkFaros.IsToggled = (bool)r["Faros"];
                    chkDefensa.IsToggled = (bool)r["Defensa"];
                    chkEmblema.IsToggled = (bool)r["Emblema"];
                    chkMolduras.IsToggled = (bool)r["Molduras"];
                    chkCofre.IsToggled = (bool)r["Cofre"];
                    //buscamos derecho
                    r = InventSelDerecho(IDInventario).Tables[0].Rows[0];
                    chkAletaD.IsToggled = (bool)r["aleta"];
                    chkTapLlantD.IsToggled = (bool)r["taponllantas"];
                    chkCrisPuertD.IsToggled = (bool)r["cristalpuertas"];
                    chkTapGasD.IsToggled = (bool)r["tapongasolina"];
                    chkManijasD.IsToggled = (bool)r["Manijas"];
                    chkReflejantesD.IsToggled = (bool)r["reflejantes"];
                    chkPuertasD.IsToggled = (bool)r["puertas"];
                    chkEmblemasD.IsToggled = (bool)r["emblema"];
                    chkEspLatD.IsToggled = (bool)r["espejolateral"];
                    chkLoderasD.IsToggled = (bool)r["loderas"];
                    chkMoldurasD.IsToggled = (bool)r["Molduras"];
                    chkChapD.IsToggled = (bool)r["chapaspuertas"];
                    chkLlantasD.IsToggled = (bool)r["llantas"];
                    //buscamos izquierdo
                    r = InventSelIzquierdo(IDInventario).Tables[0].Rows[0];
                    chkAletaI.IsToggled = (bool)r["aleta"];
                    chkTapLlantI.IsToggled = (bool)r["taponllantas"];
                    chkCrisPuertI.IsToggled = (bool)r["cristalpuertas"];
                    chkTapGasI.IsToggled = (bool)r["tapongasolina"];
                    chkManijasI.IsToggled = (bool)r["Manijas"];
                    chkReflejantesI.IsToggled = (bool)r["reflejantes"];
                    chkPuertasI.IsToggled = (bool)r["puertas"];
                    chkEmblemasI.IsToggled = (bool)r["emblema"];
                    chkEspLatI.IsToggled = (bool)r["espejolateral"];
                    chkLoderasI.IsToggled = (bool)r["loderas"];
                    chkMoldurasI.IsToggled = (bool)r["Molduras"];
                    chkChapI.IsToggled = (bool)r["chapaspuertas"];
                    chkLlantasI.IsToggled = (bool)r["llantas"];
                    //buscamos trasera
                    r = InventSelExtTrasera(IDInventario).Tables[0].Rows[0];
                    chkMedallon.IsToggled = (bool)r["medallon"];
                    chkCalaveras.IsToggled = (bool)r["micascalavera"];
                    chkDefensaB.IsToggled = (bool)r["defensa"];
                    chkEmblemasB.IsToggled = (bool)r["emblema"];
                    chkChapaB.IsToggled = (bool)r["chapacajuela"];
                    chkMoldurasB.IsToggled = (bool)r["molduras"];
                    chkHerramientas.IsToggled = (bool)r["herramienta"];
                    chkGato.IsToggled = (bool)r["gato"];
                    chkRefaccion.IsToggled = (bool)r["llantarefaccion"];
                    chkPortaPlaca.IsToggled = (bool)r["portaplaca"];
                    chkLimpiadorB.IsToggled = (bool)r["limpiador"];
                    chkExtintor.IsToggled = (bool)r["extintor"];
                    chkAlfombraB.IsToggled = (bool)r["alfombra"];
                    //buscamos interior
                    r = InventSelInterior(IDInventario).Tables[0].Rows[0];
                    chkRadio.IsToggled = (bool)r["radio"];
                    chkCD.IsToggled = (bool)r["cd"];
                    chkEqualizador.IsToggled = (bool)r["ecualizador"];
                    chkReloj.IsToggled = (bool)r["reloj"];
                    chkRetrovisor.IsToggled = (bool)r["espejoretrovisor"];
                    chkMiseras.IsToggled = (bool)r["miseras"];
                    chkEncendedor.IsToggled = (bool)r["encendedor"];
                    chkCenicero.IsToggled = (bool)r["cenicero"];
                    chkTapetes.IsToggled = (bool)r["tapetes"];
                    chkBocinas.IsToggled = (bool)r["bocinas"];
                    chkConsolas.IsToggled = (bool)r["consola"];
                    chkSeguros.IsToggled = (bool)r["seguros"];
                    chkCinturones.IsToggled = (bool)r["cinturones"];
                    chkPlafon.IsToggled = (bool)r["plafon"];
                    //buscamos motor
                    r = InventSelMotor(IDInventario).Tables[0].Rows[0];
                    chkAcumulador.IsToggled = (bool)r["Acumulador"];
                    chkTaponAgua.IsToggled = (bool)r["taponagua"];
                    chkTaponAceite.IsToggled = (bool)r["taponaceite"];
                    chkAlternador.IsToggled = (bool)r["alternador"];
                    chkMarcha.IsToggled = (bool)r["marcha"];
                    chkBayonetas.IsToggled = (bool)r["bayonetas"];
                    chkCompresor.IsToggled = (bool)r["compresoraire"];
                    chkLicuadora.IsToggled = (bool)r["licuadora"];
                    chkComputadoras.IsToggled = (bool)r["computadoras"];
                    chkFiltro.IsToggled = (bool)r["filtro"];
                    chkDepositoAgua.IsToggled = (bool)r["depositoagua"];
                    chkBandas.IsToggled = (bool)r["bandas"];
                    chkMangueras.IsToggled = (bool)r["mangueras"];
                    chkVarios.IsToggled = (bool)r["varios"];
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("FIX",ex.Message,"OK");
            }
        }
        private DataSet Ejecuta(string xQuery)
        {
            string xConnstr = ConnStr;
            if (this._Ciudad == 2)
                xConnstr = ConnStr2;
            SqlDataAdapter xAda = new SqlDataAdapter(xQuery, xConnstr);
            DataSet ds = new DataSet();
            xAda.Fill(ds);
            return ds;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            BuscaDatos();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                BuscaDatos();
                return;
            }
        }
        public void InventInsMotor(int IDInventario, int IDVehiculo, int Acumulador, int TaponAgua, int TaponAceite,
           int Alternador, int Marcha, int Bayonetas, int CompresorAire, int Licuadora, int Computadoras, int Filtro, int DepositoAgua,
           int Bandas, int Mangueras, int Varios, int LastUser)
        {
            string s = "delete porsh_admin.det_motor where IDInventario=" + IDInventario.ToString() + ";" +
                "INSERT INTO porsh_admin.det_motor (IDInventario,IDVehiculo,Acumulador,TaponAgua,TaponAceite," +
                "Alternador,Marcha,Bayonetas,CompresorAire,Licuadora,Computadoras,Filtro,DepositoAgua," +
                "Bandas,Mangueras,Varios,LastUser) VALUES (" + IDInventario.ToString() + "," + IDVehiculo.ToString() + "," +
                Acumulador.ToString() + "," + TaponAgua.ToString() + "," + TaponAceite.ToString() + "," + Alternador.ToString() + "," + Marcha.ToString() + "," +
                Bayonetas.ToString() + "," + CompresorAire.ToString() + "," + Licuadora.ToString() + "," +
                    Computadoras.ToString() + "," + Filtro.ToString() + "," + DepositoAgua.ToString() + "," + Bandas.ToString() + "," +
                    Mangueras.ToString() + "," + Varios.ToString() + "," + LastUser.ToString() + ")";
            Ejecuta(s);
        }
        public void InventInsExtTrasera(int IDInventario, int IDVehiculo, int Medallon, int MicasCalavera, int Defensa, int Emblema,
          int ChapaCajuela, int Molduras, int Herramienta, int Gato, int LlantaRefaccion, int PortaPlaca, int Limpiador, int Extintor, int Alfombra, int LastUser)
        {
            string s = "delete porsh_admin.det_ExtTrasera where IDInventario=" + IDInventario.ToString() + ";" +
                "INSERT INTO porsh_admin.det_ExtTrasera (IDInventario,IDVehiculo,Medallon,MicasCalavera," +
                "Defensa,Emblema,ChapaCajuela,Molduras,Herramienta,Gato,LlantaRefaccion,PortaPlaca," +
                "Limpiador,Extintor,Alfombra,LastUser) VALUES (" + IDInventario.ToString() + "," + IDVehiculo.ToString() + "," + Medallon.ToString() +
                "," + MicasCalavera.ToString() + "," + Defensa.ToString() + "," + Emblema.ToString() + "," + ChapaCajuela.ToString() + "," +
                Molduras.ToString() + "," + Herramienta.ToString() + "," + Gato + "," + LlantaRefaccion.ToString() + "," + PortaPlaca.ToString() +
                "," + Limpiador.ToString() + "," + Extintor.ToString() + "," + Alfombra.ToString() + "," + LastUser.ToString() + ")";
            Ejecuta(s);
        }
        public void InventInsInterior(int IDInventario, int IDVehiculo, int Radio, int CD, int Ecualizador, int Reloj, int EspejoRetrovisor,
           int Miseras, int Encendedor, int Cenicero, int Tapetes, int Bocinas, int Consola, int Seguros, int Cinturones, int Plafon, int LastUser)
        {
            string s = "delete porsh_admin.det_Interior where IDInventario=" + IDInventario.ToString() + ";" +
                "INSERT INTO porsh_admin.det_Interior (IDInventario,IDVehiculo,Radio,CD,Ecualizador,Reloj," +
                "EspejoRetrovisor,Miseras,Encendedor,Cenicero,Tapetes,Bocinas,Consola,Seguros,Cinturones,Plafon,LastUser) VALUES (" +
                IDInventario.ToString() + "," + IDVehiculo.ToString() + "," + Radio.ToString() + "," + CD.ToString() + "," + Ecualizador.ToString() +
                "," + Reloj.ToString() + "," + EspejoRetrovisor.ToString() + "," + Miseras.ToString() + "," + Encendedor.ToString() + "," +
                Cenicero.ToString() + "," + Tapetes.ToString() + "," + Bocinas.ToString() + "," + Consola.ToString() + "," + Seguros.ToString() +
                "," + Cinturones.ToString() + "," + Plafon.ToString() + "," + LastUser.ToString() + ")";
            Ejecuta(s);
        }
        public void InventInsDelanteroFrontal(int IDInventario, int IDVehiculo, int Parabrisas, int Limpiadores, int HulesLimpiadores, int Antena,
           int Defensa, int Molduras, int Parrilla, int Portaplaca, int Biceles, int Faros, int Emblema, int Cofre, int LastUser)
        {
            string s = "delete porsh_admin.det_DelanteroFrontal where IDInventario=" + IDInventario.ToString() + ";" +
            "INSERT INTO porsh_admin.det_DelanteroFrontal (IDInventario,IDVehiculo,Parabrisas,Limpiadores," +
            "HulesLimpiadores,Antena,Defensa,Molduras,Parrilla,Portaplaca,Biceles,Faros,Emblema,Cofre,LastUser) VALUES (" + IDInventario.ToString() +
            "," + IDVehiculo.ToString() + "," + Parabrisas.ToString() + "," + Limpiadores.ToString() + "," + HulesLimpiadores.ToString() + "," +
            Antena.ToString() + "," + Defensa.ToString() + "," + Molduras.ToString() + "," + Parrilla.ToString() + "," + Portaplaca.ToString() + "," +
            Biceles.ToString() + "," + Faros.ToString() + "," + Emblema.ToString() + "," + Cofre.ToString() + "," + LastUser.ToString() + ")";
            Ejecuta(s);
        }
        public void InventInsCostadoIzq(int IDInventario, int IDVehiculo, int Aleta, int CristalPuertas, int Manijas, int Puertas, int EspejoLateral,
           int Molduras, int Llantas, int TaponLlantas, int TaponGasolina, int Reflejantes, int Emblema, int Loderas, int ChapasPuertas, int LastUser)
        {
            string s = "delete porsh_admin.det_CostadoIzquierdo where IDInventario=" + IDInventario.ToString() + ";" +
                "INSERT INTO porsh_admin.det_CostadoIzquierdo (IDInventario,IDVehiculo,Aleta,CristalPuertas,Manijas,Puertas," +
                "EspejoLateral,Molduras,Llantas,TaponLlantas,TaponGasolina,Reflejantes,Emblema,Loderas,ChapasPuertas,LastUser) VALUES (" +
                IDInventario.ToString() + "," + IDVehiculo.ToString() + "," + Aleta.ToString() + "," + CristalPuertas.ToString() + "," +
                Manijas.ToString() + "," + Puertas.ToString() + "," + EspejoLateral.ToString() + "," + Molduras.ToString() + "," +
                Llantas.ToString() + "," + TaponLlantas.ToString() + "," + TaponGasolina.ToString() + "," + Reflejantes.ToString() + "," +
                Emblema.ToString() + "," + Loderas.ToString() + "," + ChapasPuertas.ToString() + "," + LastUser.ToString() + ")";
            Ejecuta(s);
        }
        public void InventInsCostadoDer(int IDInventario, int IDVehiculo, int Aleta, int CristalPuertas, int Manijas, int Puertas, int EspejoLateral,
          int Molduras, int Llantas, int TaponLlantas, int TaponGasolina, int Reflejantes, int Emblema, int Loderas, int ChapasPuertas, int LastUser)
        {
            string s = "delete porsh_admin.det_CostadoDerecho where IDInventario=" + IDInventario.ToString() + ";" +
                "INSERT INTO porsh_admin.det_CostadoDerecho (IDInventario,IDVehiculo,Aleta,CristalPuertas,Manijas,Puertas," +
                "EspejoLateral,Molduras,Llantas,TaponLlantas,TaponGasolina,Reflejantes,Emblema,Loderas,ChapasPuertas,LastUser) VALUES (" +
                IDInventario.ToString() + "," + IDVehiculo.ToString() + "," + Aleta.ToString() + "," + CristalPuertas.ToString() + "," +
                Manijas.ToString() + "," + Puertas.ToString() + "," + EspejoLateral.ToString() + "," + Molduras.ToString() + "," +
                Llantas.ToString() + "," + TaponLlantas.ToString() + "," + TaponGasolina.ToString() + "," + Reflejantes.ToString() + "," +
                Emblema.ToString() + "," + Loderas.ToString() + "," + ChapasPuertas.ToString() + "," + LastUser.ToString() + ")";
            Ejecuta(s);
        }
        public void InventarioMainUpd(int IDInventario, int Automatico, int Cilindros, string KmInicial, string KmFin, string CondicionesLlantas,
            string MarcaLlantas, string MedidaLlantas, string Preexistente, string Gasolina, string Comentarios, int IDUser)
        {
            string s = "update porsh_admin.tbl_InventarioVehiculo set Automatico=" + Automatico.ToString() + ",Cilindros=" + Cilindros.ToString() +
                ",KmInicial='" + KmInicial.ToString() + "',KmFin='" + KmFin.ToString() + "',CondicionesLlantas='" + CondicionesLlantas.ToString() +
                "',MarcaLlantas='" + MarcaLlantas.ToString() + "',MedidaLlantas='" + MedidaLlantas.ToString() + "',Preexistente='" +
            Preexistente.ToString() + "',Gasolina='" + Gasolina.ToString() + "',Comentarios='" + Comentarios.ToString() + "',LastUser=" +
            IDUser.ToString() + ", lastdate=dateadd(hh,-2,getdate()) where idInventario =" + IDInventario.ToString();
            Ejecuta(s);
        }
        public DataSet InventarioMainIns(int IDVehiculo, int Automatico, int Cilindros, string KmInicial, string KmFin, string CondicionesLlantas,
            string MarcaLlantas, string MedidaLlantas, string Preexistente, string Gasolina, string Comentarios, int IDUser)
        {
            string s = "INSERT INTO porsh_admin.tbl_InventarioVehiculo (IDVehiculo,Automatico,Cilindros,KmInicial,KmFin,CondicionesLlantas, " +
                "MarcaLlantas,MedidaLlantas,Preexistente,Gasolina,Comentarios,LastUser) values (" + IDVehiculo.ToString() + "," + Automatico.ToString() +
                "," + Cilindros.ToString() + ",'" + KmInicial + "','" + KmFin + "','" + CondicionesLlantas + "','" + MarcaLlantas + "','" +
                MedidaLlantas + "','" + Preexistente + "','" + Gasolina + "','" + Comentarios + "'," + IDUser.ToString() +
                ");select @@identity as ElID";
            return Ejecuta(s);
        }
        public void InventUpdPDF(int IDInventario, byte[] xPDF)
        {
            SqlConnection cnx;
            if (_Ciudad == 1)
                cnx = new SqlConnection(ConnStr);
            else
                cnx = new SqlConnection(ConnStr2);
            string s = "update porsh_admin.tbl_InventarioVehiculo set xPDF=@xImage where IDInventario=" + IDInventario.ToString();
            SqlCommand cmd = new SqlCommand(s, cnx);
            SqlParameter fd = new SqlParameter("@xImage", SqlDbType.Image);
            fd.Value = xPDF;
            cnx.Open();
            cmd.Parameters.Add(fd);
            cmd.ExecuteNonQuery();
            cnx.Close();
            cnx = null;
            cmd = null;
            fd = null;
        }

        public void InventarioUpdFirma(int idInventario, byte[] Firma)
        {
            SqlConnection cnx;
            if (_Ciudad == 1)
                cnx = new SqlConnection(ConnStr);
            else
                cnx = new SqlConnection(ConnStr2);
            string s = "update porsh_admin.tbl_InventarioVehiculo set FirmaCliente=@xImage where IDInventario=" + idInventario.ToString();
            SqlCommand cmd = new SqlCommand(s, cnx);
            SqlParameter fd = new SqlParameter("@xImage", SqlDbType.Image);
            fd.Value = Firma;
            cnx.Open();
            cmd.Parameters.Add(fd);
            cmd.ExecuteNonQuery();
            cnx.Close();
            cnx = null;
            cmd = null;
            fd = null;
        }
        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            try
            {
                Usuarios x= (Usuarios) cmbUsuario.SelectedItem;
                if (!SignDam.IsBlank && !SignFirmaCliente.IsBlank && !SignFirmaTaller.IsBlank && !SignGas.IsBlank)
                {
                    Stream sDam = await SignDam.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Jpeg,false,true);
                    Stream sCliente = await SignFirmaCliente.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Jpeg, false, true);
                    Stream sTaller = await SignFirmaTaller.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Jpeg, false, true);
                    Stream sGas = await SignGas.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Jpeg, false, true);


                    byte[] bDam; byte[] bCliente; byte[] bTaller; byte[] bGas;
                    using (var mems = new MemoryStream())
                    {
                        sDam.CopyTo(mems);
                        bDam = mems.ToArray();
                    }
                    using (var mems = new MemoryStream())
                    {
                        sCliente.CopyTo(mems);
                        bCliente = mems.ToArray();
                    }
                    using (var mems = new MemoryStream())
                    {
                        sTaller.CopyTo(mems);
                        bTaller = mems.ToArray();
                    }
                    using (var mems = new MemoryStream())
                    {
                        sGas.CopyTo(mems);
                        bGas = mems.ToArray();
                    }

                    if (IDInventario == 0)
                    {
                        DataSet d = InventarioMainIns(iVehiculo, cmbTrans.SelectedIndex, int.Parse(cmbCilindros.SelectedItem.ToString()),
                            txtKMIni.Text, txtKMFin.Text, txtCondLlantas.Text, txtMarcaLlantas.Text, txtMedidaLLantas.Text,
                            txtDamPree.Text, "", txtComentarios.Text, x.ID);
                        IDInventario = int.Parse(d.Tables[0].Rows[0][0].ToString());
                    }
                    else
                    {
                        InventarioMainUpd(IDInventario, cmbTrans.SelectedIndex, int.Parse(cmbCilindros.SelectedItem.ToString()),
                            txtKMIni.Text, txtKMFin.Text, txtCondLlantas.Text, txtMarcaLlantas.Text, txtMedidaLLantas.Text,
                            txtDamPree.Text, "", txtComentarios.Text, x.ID);
                    }

                    InventarioUpdFirma(IDInventario, bCliente);
                    InventInsDelanteroFrontal(IDInventario, iVehiculo, chkParabrisas.IsToggled ? 1 : 0, chkBLimp.IsToggled ? 1 : 0,
                        chkHulesLimp.IsToggled ? 1 : 0, chkAntena.IsToggled ? 1 : 0, chkDefensa.IsToggled ? 1 : 0,
                        chkMolduras.IsToggled ? 1 : 0, chkParrilla.IsToggled ? 1 : 0, chkPortaPlaca.IsToggled ? 1 : 0,
                        chkBiceles.IsToggled ? 1 : 0, chkFaros.IsToggled ? 1 : 0, chkEmblema.IsToggled ? 1 : 0,
                        chkCofre.IsToggled ? 1 : 0, x.ID);
                    InventInsCostadoDer(IDInventario, iVehiculo, chkAletaD.IsToggled ? 1 : 0, chkCrisPuertD.IsToggled ? 1 : 0,
                        chkManijasD.IsToggled ? 1 : 0, chkPuertasD.IsToggled ? 1 : 0, chkEspLatD.IsToggled ? 1 : 0,
                        chkMoldurasD.IsToggled ? 1 : 0, chkLlantasD.IsToggled ? 1 : 0, chkTapLlantD.IsToggled ? 1 : 0,
                        chkTapGasD.IsToggled ? 1 : 0, chkReflejantesD.IsToggled ? 1 : 0, chkEmblemasD.IsToggled ? 1 : 0,
                        chkLoderasD.IsToggled ? 1 : 0, chkChapD.IsToggled ? 1 : 0, x.ID);
                    InventInsCostadoIzq(IDInventario, iVehiculo, chkAletaI.IsToggled ? 1 : 0, chkCrisPuertI.IsToggled ? 1 : 0,
                        chkManijasI.IsToggled ? 1 : 0, chkPuertasI.IsToggled ? 1 : 0, chkEspLatI.IsToggled ? 1 : 0,
                        chkMoldurasI.IsToggled ? 1 : 0, chkLlantasI.IsToggled ? 1 : 0, chkTapLlantI.IsToggled ? 1 : 0,
                        chkTapGasI.IsToggled ? 1 : 0, chkReflejantesI.IsToggled ? 1 : 0, chkEmblemasI.IsToggled ? 1 : 0,
                        chkLoderasI.IsToggled ? 1 : 0, chkChapI.IsToggled ? 1 : 0, x.ID);
                    InventInsExtTrasera(IDInventario, iVehiculo, chkMedallon.IsToggled ? 1 : 0, chkCalaveras.IsToggled ? 1 : 0,
                        chkDefensaB.IsToggled ? 1 : 0, chkEmblemasB.IsToggled ? 1 : 0, chkChapaB.IsToggled ? 1 : 0,
                        chkMoldurasB.IsToggled ? 1 : 0, chkHerramientas.IsToggled ? 1 : 0, chkGato.IsToggled ? 1 : 0,
                        chkRefaccion.IsToggled ? 1 : 0, chkPortaPlaca.IsToggled ? 1 : 0, chkLimpiadorB.IsToggled ? 1 : 0,
                        chkExtintor.IsToggled ? 1 : 0, chkAlfombraB.IsToggled ? 1 : 0, x.ID);
                    InventInsInterior(IDInventario, iVehiculo, chkRadio.IsToggled ? 1 : 0, chkCD.IsToggled ? 1 : 0,
                        chkEqualizador.IsToggled ? 1 : 0, chkReloj.IsToggled ? 1 : 0, chkRetrovisor.IsToggled ? 1 : 0,
                        chkMiseras.IsToggled ? 1 : 0, chkEncendedor.IsToggled ? 1 : 0, chkCenicero.IsToggled ? 1 : 0,
                        chkTapetes.IsToggled ? 1 : 0, chkBocinas.IsToggled ? 1 : 0, chkConsolas.IsToggled ? 1 : 0,
                        chkSeguros.IsToggled ? 1 : 0, chkCinturones.IsToggled ? 1 : 0, chkPlafon.IsToggled ? 1 : 0, x.ID);
                    InventInsMotor(IDInventario, iVehiculo, chkAcumulador.IsToggled ? 1 : 0, chkTaponAgua.IsToggled ? 1 : 0, chkTaponAceite.IsToggled ? 1 : 0,
                        chkAlternador.IsToggled ? 1 : 0, chkMarcha.IsToggled ? 1 : 0, chkBayonetas.IsToggled ? 1 : 0, chkCompresor.IsToggled ? 1 : 0,
                        chkLicuadora.IsToggled ? 1 : 0, chkComputadoras.IsToggled ? 1 : 0, chkFiltro.IsToggled ? 1 : 0,
                        chkDepositoAgua.IsToggled ? 1 : 0, chkBandas.IsToggled ? 1 : 0, chkMangueras.IsToggled ? 1 : 0, chkVarios.IsToggled ? 1 : 0, x.ID);
                    if (GeneraPDF(IDInventario, bDam, bCliente, bTaller, bGas))
                        Limpia();
                    else
                        DisplayAlert("FIX", "Ocurrio un error al guardar la informacion", "OK");
                }
                else
                {
                    DisplayAlert("FIX", "Debe capturar las firmas, la medida de la gasolina y los daños", "OK");
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("FIX",ex.Message,"OK");
            }
        }
        public DataSet getVehiculo(int IDVehiculo)
        {
            string s = "select * from porsh_admin.tbl_vehicles where IDVehicles=" + IDVehiculo.ToString();
            return Ejecuta(s);
        }
        public DataSet getCustomerByVehicle(int idVehicle)
        {
            string s = "select a.* from porsh_admin.tbl_customers a, porsh_admin.tbl_vehicles b " +
            "where a.IDCustomer = b.IDCustomer and IDVehicles=" + idVehicle.ToString();
            return Ejecuta(s);
        }
        private void Limpia()
        {
            SignFirmaTaller.Clear();
            SignGas.Clear();
            SignFirmaCliente.Clear();
            SignDam.Clear();
            txtComentarios.Text = "";txtCondLlantas.Text = "";txtDamPree.Text = "";txtID.Text = "";
            txtKMFin.Text = "";txtKMIni.Text = "";txtMarcaLlantas.Text = "";txtMedidaLLantas.Text = "";
            txtID.Focus();

            chkAcumulador.IsToggled = true;chkAletaD.IsToggled = true;chkAletaI.IsToggled = true;chkAlfombraB.IsToggled = true;
            chkAlternador.IsToggled = true;chkAntena.IsToggled = true;chkBandas.IsToggled = true;chkBayonetas.IsToggled = true;
            chkBiceles.IsToggled = true;chkBLimp.IsToggled = true;chkBocinas.IsToggled = true;chkCalaveras.IsToggled = true;
            chkCD.IsToggled = true;chkCenicero.IsToggled = true;chkChapaB.IsToggled = true;chkChapD.IsToggled = true;
            chkChapI.IsToggled = true;chkCinturones.IsToggled = true;chkCofre.IsToggled = true;chkCompresor.IsToggled = true;
            chkComputadoras.IsToggled = true;chkConsolas.IsToggled = true;chkCrisPuertD.IsToggled = true;chkCrisPuertI.IsToggled = true;
            chkDefensa.IsToggled = true;chkDefensaB.IsToggled = true;chkDepositoAgua.IsToggled = true;chkEmblema.IsToggled = true;
            chkEmblemasB.IsToggled = true;chkEmblemasD.IsToggled = true;chkEmblemasI.IsToggled = true;chkEncendedor.IsToggled = true;
            chkEqualizador.IsToggled = true;chkEspLatD.IsToggled = true;chkEspLatI.IsToggled = true;chkExtintor.IsToggled = true;
            chkFaros.IsToggled = true;chkFiltro.IsToggled = true;chkGato.IsToggled = true;chkHerramientas.IsToggled = true;
            chkHulesLimp.IsToggled = true;chkLicuadora.IsToggled = true;chkLimpiadorB.IsToggled = true;chkLlantasD.IsToggled = true;
            chkLlantasI.IsToggled = true;chkLoderasD.IsToggled = true;chkLoderasI.IsToggled = true;chkMangueras.IsToggled = true;
            chkManijasD.IsToggled = true;chkManijasI.IsToggled = true;chkMarcha.IsToggled = true;chkMedallon.IsToggled = true;
            chkMiseras.IsToggled = true;chkMolduras.IsToggled = true;chkMoldurasB.IsToggled = true;chkMoldurasD.IsToggled = true;
            chkMoldurasI.IsToggled = true;chkParabrisas.IsToggled = true;chkParrilla.IsToggled = true;chkPlacaB.IsToggled = true;
            chkPlafon.IsToggled = true;chkPortaPlaca.IsToggled = true;chkPuertasD.IsToggled = true;chkPuertasI.IsToggled = true;
            chkRadio.IsToggled = true;chkRefaccion.IsToggled = true;chkReflejantesD.IsToggled = true;chkReflejantesI.IsToggled = true;
            chkReloj.IsToggled = true;chkRetrovisor.IsToggled = true;chkSeguros.IsToggled = true;chkTapetes.IsToggled = true;
            chkTapGasD.IsToggled = true;chkTapGasI.IsToggled = true;chkTapLlantD.IsToggled = true;chkTapLlantI.IsToggled = true;
            chkTaponAceite.IsToggled = true;chkTaponAgua.IsToggled = true;chkVarios.IsToggled = true;
            cmbCilindros.SelectedIndex = 0;
            cmbTrans.SelectedIndex = 0;
            lblDatosVehculo.Text = "";
            DisplayAlert("FIX", "Se almaceno la informacion", "OK");
        }
        private bool GeneraPDF(int iInventario, byte[] bDam, byte[] bCliente, byte[] bTaller, byte[] bGas)
        {
            using (MemoryStream output = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4);
                PdfWriter writer = PdfWriter.GetInstance(doc, output);
                iTextSharp.text.Image logo =  iTextSharp.text.Image.GetInstance("http://fix.fungor.net/img/logoinv.jpg");
                logo.ScaleAbsoluteHeight(100);
                logo.ScaleAbsoluteWidth(100); logo.Border = 0;
                iTextSharp.text.Image logo2 = iTextSharp.text.Image.GetInstance("http://fix.fungor.net/img/logoinv2.jpg");
                logo2.ScaleAbsoluteHeight(100); logo2.Border = 0;
                logo2.ScaleAbsoluteWidth(100);
                doc.Open();

                //gasolina y daños

                byte[] imGas = bGas;
                byte[] imDam = bDam;


                //ImgWork fT = new ImgWork();
                //iTextSharp.text.Image BackGas = iTextSharp.text.Image.GetInstance(fT.imageToByteArray(fT.resizeImage(BackGas2, new System.Drawing.Size(200, 200))));

                //ponemos palabra inventario
                PdfPTable Principal = new PdfPTable(1);
                PdfPCell Celda;
                var TitleFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 14,iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                var boldFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 8, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
                var normalFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 9, BaseColor.BLACK);
                var smallFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, BaseColor.BLACK);
                var boldsmallFont = FontFactory.GetFont(FontFactory.TIMES_ROMAN, 7, iTextSharp.text.Font.BOLD, BaseColor.BLACK);


                Principal.DefaultCell.Border = 0;
                Principal.WidthPercentage = 100;
                Principal.HorizontalAlignment =iTextSharp.text.Element.ALIGN_CENTER;
                Celda = new PdfPCell(new Phrase("INVENTARIO", TitleFont));
                Celda.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                Celda.Border = 0;
                Principal.AddCell(Celda);

                #region tabla de titulo
                PdfPTable tHead = new PdfPTable(3);
                float[] widths = new float[] { 10, 80, 10 };
                tHead.SetWidths(widths);
                tHead.AddCell(logo);
                PdfPTable tTitulo = new PdfPTable(2);

                Phrase fr = new Phrase();
                fr.Add(new Chunk("F: ", boldFont));
                fr.Add(new Chunk(IDInventario.ToString(), normalFont));
                tTitulo.AddCell(fr);
                fr = new Phrase();
                fr.Add(new Chunk("F: ", boldFont));
                fr.Add(new Chunk(DateTime.Now.ToString("dd/MM/yyyy"), normalFont));
                tTitulo.AddCell(fr);
                string Domicilio = "BLVD. CUAUHTEMOC SUR #2920 COL. DAVILA, TIJUANA BAJA CALIFORNIA, MEXICO";
                string Tel = "(664) 686 2058, (664) 686 4153";
                if (_Ciudad == 2)
                {
                    Domicilio = "BLVD. LÁZARO CÁRDENAS KM 2.5 COL. RANCHO LA BODEGA, MEXICALI B. C.";
                    Tel = "568 86 38, 568 88 81, 555 00 97";
                }

                fr = new Phrase();
                fr.Add(new Chunk("D: ", boldFont));
                fr.Add(new Chunk(Domicilio, normalFont));
                Celda = new PdfPCell(fr);
                Celda.Colspan = 2;
                tTitulo.AddCell(Celda);
                fr = new Phrase();
                fr.Add(new Chunk("T: ", boldFont));
                fr.Add(new Chunk(Tel, normalFont));
                tTitulo.AddCell(fr);
                fr = new Phrase();
                fr.Add(new Chunk("C: ", boldFont));
                fr.Add(new Chunk("info@tallerfix.com", normalFont));
                tTitulo.AddCell(fr);
                tHead.AddCell(tTitulo);
                tHead.AddCell(logo2);
                Principal.AddCell(tHead);
                #endregion
                //renglon en blanco
                Celda = new PdfPCell(new Phrase("", TitleFont));
                Celda.Border = 0;
                Principal.AddCell(Celda);
                Celda = new PdfPCell(new Phrase("", TitleFont));
                Celda.Border = 0;
                Principal.AddCell(Celda);

                #region Datos Principales
                tHead = new PdfPTable(4);
                widths = new float[] { 25, 25, 25, 25 };
                tHead.SetWidths(widths);

                DataSet d = getVehiculo(iVehiculo);
                DataRow r = d.Tables[0].Rows[0];
                switch ((int)r["Asegurado"])
                {
                    case 1:
                        Celda = new PdfPCell(new Phrase("ASEGURADO", normalFont));
                        break;
                    case 2:
                        Celda = new PdfPCell(new Phrase("TERCERO", normalFont));
                        break;
                    case 3:
                        Celda = new PdfPCell(new Phrase("PARTICULAR", normalFont));
                        break;
                }
                tHead.AddCell(Celda);
                DataRow rr = getCustomerByVehicle(iVehiculo).Tables[0].Rows[0];
                fr = new Phrase();
                fr.Add(new Chunk("NOMBRE: ", boldFont));
                fr.Add(new Chunk(rr["Name"].ToString(), normalFont));
                Celda = new PdfPCell(fr);
                Celda.Colspan = 2;
                tHead.AddCell(Celda);
                fr = new Phrase();
                fr.Add(new Chunk("TELEFONO: ", boldFont));
                fr.Add(new Chunk(rr["phone"].ToString(), normalFont));
                tHead.AddCell(fr);

                //2do Renglon
                fr = new Phrase();
                fr.Add(new Chunk("CORREO: ", boldFont));
                fr.Add(new Chunk(rr["Email"].ToString(), normalFont));
                Celda = new PdfPCell(fr);
                Celda.Colspan = 2;
                tHead.AddCell(Celda);
                Celda = new PdfPCell(new Phrase("FECHA DE SINIESTRO ", smallFont));
                tHead.AddCell(Celda);
                Celda = new PdfPCell(new Phrase("FECHA DE ENVIO A DEPTO DE VALUACION ", smallFont));
                tHead.AddCell(Celda);

                //3er Renglon
                fr = new Phrase();
                fr.Add(new Chunk("MARCA: ", boldFont));
                fr.Add(new Chunk(r["Brand"].ToString(), normalFont));
                Celda = new PdfPCell(fr);
                tHead.AddCell(Celda);
                fr = new Phrase();
                fr.Add(new Chunk("MODELO: ", boldFont));
                fr.Add(new Chunk(r["Year"].ToString(), normalFont));
                Celda = new PdfPCell(fr);
                tHead.AddCell(Celda);
                fr = new Phrase();
                fr.Add(new Chunk("VIN: ", boldFont));
                fr.Add(new Chunk(r["serie"].ToString(), normalFont));
                Celda = new PdfPCell(fr);
                tHead.AddCell(Celda);
                fr = new Phrase();
                fr.Add(new Chunk("COLOR: ", boldFont));
                fr.Add(new Chunk(r["Color"].ToString(), normalFont));
                Celda = new PdfPCell(fr);
                tHead.AddCell(Celda);

                fr = new Phrase();
                fr.Add(new Chunk("PLACAS: ", boldFont));
                fr.Add(new Chunk(r["Plates"].ToString(), normalFont));
                Celda = new PdfPCell(fr);
                tHead.AddCell(Celda);
                fr = new Phrase(cmbTrans.SelectedItem.ToString().ToUpper(), boldFont);
                Celda = new PdfPCell(fr);
                tHead.AddCell(Celda);
                fr = new Phrase();
                fr.Add(new Chunk("CILINDROS: ", boldFont));
                fr.Add(new Chunk(cmbCilindros.SelectedItem.ToString(), normalFont));
                Celda = new PdfPCell(fr);
                tHead.AddCell(Celda);
                fr = new Phrase(r["Type"].ToString(), boldFont);
                Celda = new PdfPCell(fr);
                tHead.AddCell(Celda);


                fr = new Phrase();
                fr.Add(new Chunk("VERSION: ", boldFont));
                fr.Add(new Chunk(r["Model"].ToString(), normalFont));
                Celda = new PdfPCell(fr);
                Celda.Colspan = 2;
                tHead.AddCell(Celda);
                fr = new Phrase();
                fr.Add(new Chunk("KM I: ", boldFont));
                fr.Add(new Chunk(txtKMIni.Text, normalFont));
                Celda = new PdfPCell(fr);
                tHead.AddCell(Celda);
                fr = new Phrase();
                fr.Add(new Chunk("KM F: ", boldFont));
                fr.Add(new Chunk(txtKMFin.Text, normalFont));
                Celda = new PdfPCell(fr);
                tHead.AddCell(Celda);

                fr = new Phrase();
                fr.Add(new Chunk("CONDICIONES ACTUALES DE LAS LLANTAS: \n", boldFont));
                fr.Add(new Chunk(txtCondLlantas.Text, normalFont));
                Celda = new PdfPCell(fr);
                Celda.Colspan = 2;
                tHead.AddCell(Celda);
                fr = new Phrase();
                fr.Add(new Chunk("MARCA DE LAS LLANTAS: \n", boldFont));
                fr.Add(new Chunk(txtMarcaLlantas.Text, normalFont));
                Celda = new PdfPCell(fr);
                tHead.AddCell(Celda);
                fr = new Phrase();
                fr.Add(new Chunk("MEDIDA DE LAS LLANTAS: \n", boldFont));
                fr.Add(new Chunk(txtMedidaLLantas.Text, normalFont));
                Celda = new PdfPCell(fr);
                tHead.AddCell(Celda);
                #endregion
                Principal.AddCell(tHead);
                Celda = new PdfPCell(new Phrase("PARTES EXTERIORES", TitleFont));
                Celda.HorizontalAlignment =iTextSharp.text.Element.ALIGN_CENTER;
                Celda.Border = 0;
                Principal.AddCell(Celda);

                #region Exteriores
                tHead = new PdfPTable(4);
                widths = new float[] { 25, 25, 25, 25 };
                tHead.SetWidths(widths);

                PdfPTable tExterior = new PdfPTable(3);
                widths = new float[] { 74, 13, 13 };
                tExterior.SetWidths(widths);

                #region Delantera Frontal
                fr = new Phrase("DELANTERA FRONTAL", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase("SI", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase("NO", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                string v1 = "", v2 = "X";
                #region Delantero Frontal
                if (chkParabrisas.IsToggled)
                {
                    v1 = "X";v2 = "";
                }
                fr = new Phrase("Parabrisas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkBLimp.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("B. Limpiadores", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkHulesLimp.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Hules Limpiadores", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkAntena.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Antena", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkDefensa.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Defensa", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkMolduras.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Molduras", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkParrilla.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Parrilla", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkPortaPlaca.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Porta Placa", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkBiceles.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Biceles", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkFaros.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Faros", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkEmblema.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Emblema", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkCofre.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Cofre", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                #endregion

                tHead.AddCell(tExterior);
                #endregion
                #region Derecho
                tExterior = new PdfPTable(3);
                widths = new float[] { 74, 13, 13 };
                tExterior.SetWidths(widths);
                fr = new Phrase("COSTADO DERECHO", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase("SI", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase("NO", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkAletaD.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Aleta", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkCrisPuertD.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Cristales Puertas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                
                v1 = ""; v2 = "X";
                if (chkManijasD.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Manijas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkPuertasD.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Puertas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkEspLatD.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Espejo Lateral", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkMoldurasD.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Molduras", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkLlantasD.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Llantas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkTapLlantD.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Tapones Llantas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkTapGasD.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Tapon de Gasolina", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkReflejantesD.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Reflejantes", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkEmblemasD.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Emblemas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkLoderasD.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Loderas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkChapD.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Chapas Puerta", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                tHead.AddCell(tExterior);
                #endregion
                #region Izquierdo
                tExterior = new PdfPTable(3);
                widths = new float[] { 74, 13, 13 };
                tExterior.SetWidths(widths);
                fr = new Phrase("COSTADO IZQUIERDO", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase("SI", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase("NO", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkAletaI.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Aleta", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkCrisPuertI.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Cristales Puertas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkManijasI.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Manijas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkPuertasI.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Puertas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkEspLatI.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Espejo Lateral", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkMoldurasI.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Molduras", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkLlantasI.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Llantas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkTapLlantI.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Tapones Llantas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkTapGasI.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Tapon de Gasolina", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkReflejantesI.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Reflejantes", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkEmblemasI.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Emblemas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkLoderasI.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Loderas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                v1 = ""; v2 = "X";
                if (chkChapI.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Chapas Puerta", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                tHead.AddCell(tExterior);
                #endregion

                #region Exterior Trasero
                tExterior = new PdfPTable(3);
                widths = new float[] { 74, 13, 13 };
                tExterior.SetWidths(widths);
                fr = new Phrase("TRASERA", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase("SI", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase("NO", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkMedallon.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Medallon", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkCalaveras.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Micas Calaveras", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkDefensaB.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Defensa", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkEmblemasB.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Emblemas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkChapaB.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Chapa Cajuela", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkMoldurasB.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Molduras", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkHerramientas.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Herramientas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkGato.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Gato", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkRefaccion.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Llanta de Refaccion", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkPortaPlaca.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Porta Placa", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkLimpiadorB.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Limpiador", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkExtintor.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Extintor", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkAlfombraB.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Alfombra", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                tHead.AddCell(tExterior);
                #endregion

                Principal.AddCell(tHead);
                #endregion

                #region Interiores
                tHead = new PdfPTable(3);
                widths = new float[] { 25, 25, 50 };
                tHead.SetWidths(widths);

                tExterior = new PdfPTable(3);
                widths = new float[] { 74, 13, 13 };
                tExterior.SetWidths(widths);

                #region Delantera Frontal
                fr = new Phrase("DELANTERA FRONTAL", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase("SI", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase("NO", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkRadio.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Radio AM o FM", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkCD.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Compact Disc", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkEqualizador.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Equalizador", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkReloj.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Reloj", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkRetrovisor.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Espejo Retrovisor", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkMiseras.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Miseras", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkEncendedor.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Encendedor", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkCenicero.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Cenicero", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkTapetes.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Juego De Tapetes", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkBocinas.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Bocinas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkConsolas.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Consolas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkSeguros.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Seguros", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkCinturones.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Cinturones De Seguridad", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkPlafon.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Plafon Central", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                tHead.AddCell(tExterior);
                #endregion
                #region Motor
                tExterior = new PdfPTable(3);
                widths = new float[] { 74, 13, 13 };
                tExterior.SetWidths(widths);
                fr = new Phrase("MOTOR", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase("SI", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase("NO", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkAcumulador.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Acumulador", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkTaponAgua.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Tapon De Agua", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkTaponAceite.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Tapon De Aceite", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkAlternador.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Alternador", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkMarcha.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Marcha", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkBayonetas.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Bayonetas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkCompresor.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Compresor De Aire", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkLicuadora.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Licuadora", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkComputadoras.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Computadoras", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkFiltro.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Filtro", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkDepositoAgua.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Deposito De Agua", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkBandas.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Bandas", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkMangueras.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Mangueras", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);


                v1 = ""; v2 = "X";
                if (chkVarios.IsToggled)
                {
                    v1 = "X"; v2 = "";
                }
                fr = new Phrase("Varios", boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v1, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);
                fr = new Phrase(v2, boldsmallFont);
                Celda = new PdfPCell(fr);
                tExterior.AddCell(Celda);

                tHead.AddCell(tExterior);
                #endregion

                iTextSharp.text.Image Damag = iTextSharp.text.Image.GetInstance(imDam);
                Damag.ScaleAbsoluteHeight(200); Damag.Border = 1;
                Damag.ScaleAbsoluteWidth(250);
                Celda = new PdfPCell(Damag);
                Celda.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                Celda.VerticalAlignment = iTextSharp.text.Element.ALIGN_MIDDLE;
                Celda.MinimumHeight = 200;
                tHead.AddCell(Celda);

                Principal.AddCell(tHead);
                #endregion
                #region Penultimo
                tHead = new PdfPTable(3);
                widths = new float[] { 25, 50, 25 };
                tHead.SetWidths(widths);
                Celda = new PdfPCell(new Phrase("DAÑOS PREEXISTENTES", boldsmallFont));
                tHead.AddCell(Celda);
                Celda = new PdfPCell(new Phrase(txtDamPree.Text + "\n" + txtComentarios.Text, smallFont));
                tHead.AddCell(Celda);



                //ImageEvent imgEvent = new ImageEvent(BackGas);
                logo2 = iTextSharp.text.Image.GetInstance(imGas);
                logo2.ScaleAbsoluteHeight(50); logo2.Border = 1;
                logo2.ScaleAbsoluteWidth(120);
                iTextSharp.text.Image Gasolinalevel = iTextSharp.text.Image.GetInstance(imGas);
                
                Celda = new PdfPCell(logo2);
                //Celda.CellEvent = imgEvent;
                //Celda.CellEvent = imgEvent2;
                // Celda.MinimumHeight = 100;
                tHead.AddCell(Celda);


                Principal.AddCell(tHead);
                #endregion
                #region Firmas


                byte[] FirmaCliente = bCliente;// Convert.FromBase64String(txtImgTemp.Text.Replace("data:image/png;base64,", ""));
                byte[] FirmaTaller = bTaller;// Convert.FromBase64String(txtFirmaTaller.Text.Replace("data:image/png;base64,", ""));
                //MemoryStream ms = new MemoryStream(FirmaCliente);
                //System.Drawing.Image xIMG = System.Drawing.Image.FromStream(ms);             

                iTextSharp.text.Image FirmaCte = iTextSharp.text.Image.GetInstance(FirmaCliente);
                FirmaCte.ScaleAbsoluteHeight(80); FirmaCte.Border = 0;
                FirmaCte.ScaleAbsoluteWidth(150);
                iTextSharp.text.Image FirmaTall = iTextSharp.text.Image.GetInstance(FirmaTaller);
                FirmaTall.ScaleAbsoluteHeight(80); FirmaTall.Border = 0;
                FirmaTall.ScaleAbsoluteWidth(150);

                tHead = new PdfPTable(3);
                widths = new float[] { 37, 37, 26 };
                tHead.SetWidths(widths);
                Celda = new PdfPCell(new Phrase("NOMBRE Y FIRMA DEL RESPONSABLE DE TALLER", boldsmallFont));
                tHead.AddCell(Celda);


                Celda = new PdfPCell(new Phrase("NOMBRE Y FIRMA DEL CLIENTE", boldsmallFont));
                tHead.AddCell(Celda);

                Celda = new PdfPCell(new Phrase("Adjuntar este volante a la factura\nde servicio para facilitar el\ntramite de pago.", boldsmallFont));
                Celda.MinimumHeight = 80; Celda.Rowspan = 2;
                Celda.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                tHead.AddCell(Celda);



                //Celda = new PdfPCell(new Phrase(_Nombre, boldsmallFont));
                Celda = new PdfPCell(FirmaTall); Celda.Border = 0;
                Celda.VerticalAlignment = iTextSharp.text.Element.ALIGN_BOTTOM; Celda.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                tHead.AddCell(Celda);


                Celda = new PdfPCell(FirmaCte); Celda.Border = 0;
                Celda.VerticalAlignment = iTextSharp.text.Element.ALIGN_BOTTOM; Celda.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                tHead.AddCell(Celda);

                Principal.AddCell(tHead);
                #endregion
                doc.Add(Principal);
                doc.Close();

                //lo pasamos a bytes para guardarlo
                byte[] xFile = output.ToArray();

                //Response.Buffer = true;
                //Response.Charset = "";
                //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //Response.ContentType = "application/octet-stream";
                //Response.AddHeader("Content-Disposition", "attachment;filename=Inventario.pdf");
                //Response.BinaryWrite(xFile);
                //Response.Flush();
                //Response.End();

                InventUpdPDF(IDInventario, xFile);


                doc.Close();
                writer.Close();

            }
            return true;
        }
    }


}
