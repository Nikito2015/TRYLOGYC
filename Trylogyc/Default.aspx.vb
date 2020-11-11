Imports Trylogyc.DAL
Imports System.IO
Imports System.Net
Imports MercadoPago.Resources
Imports MercadoPago.DataStructures.Preference
Imports MercadoPago.Common
Imports System.Globalization
Imports System.Linq
Imports System.Xml

Public Class _Default
    Inherits System.Web.UI.Page
    Dim myContext = New TrylogycContext
    Dim sumtotal As Double
    Public preferenceIDMP As String

    Protected Sub page_preinit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        If login.ValidateSessionCookie() = True Then
        Else
            Response.Redirect("~/login.aspx")
        End If

    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If login.ValidateSessionCookie() = True Then
            Me.divError.Visible = False
            If Not Page.IsPostBack Then
                If Session("IDUsuario") Is Nothing Then
                    Session("IDUsuario") = Request.Cookies("IDUsuario").Value
                    If Session("IDUsuario") Is Nothing Then
                        Session("IDUsuario") = Request.Params("IDUsuario")
                    End If
                End If

                If Session("xmlSocio") Is Nothing Then
                    Session("xmlSocio") = Request.Cookies("xmlSocio").Value
                    If Session("xmlSocio") Is Nothing Then
                        Session("xmlSocio") = Request.Params("xmlSocio")
                    End If
                End If
                Dim filtroConexiones As String = Request.Cookies("filtroConexiones").Value
                If Session("dtSocio") Is Nothing Or Session("dtSaldo") Is Nothing Then
                    RecuperarDtSociosYSaldos(filtroConexiones)
                End If

                Dim daUser As DataSet = myContext.GetUsuario(Session("IDUsuario"))
                If daUser.Tables(0).TableName = "Error" Then
                    Session("codError") = daUser.Tables(0).Rows(0).Item(0)
                    Session("txtError") = daUser.Tables(0).Rows(0).Item(1)
                    Response.Redirect("~/Error.aspx")
                Else
                    ''traer listado de socio-conexion de la tabla
                    'traer listado de conexiones para el socio
#Region "NUEVO METODO"


                    lstConexiones.Items.Insert(0, " Todas")
                    lstConexiones.Items.Item(0).Value = 0
                    Dim dtSocio As DataTable = Session("dtSocio")

                    CargarComboConexiones(dtSocio)
                    readFacturas(Session("xmlSocio"), lstConexiones.SelectedValue)
                    'Me.txtDeudatotal.Text = Format((sumtotal), "0.00")
#End Region
                End If

                GridView1.DataBind()
                'Ocultar/Mostrar columna "Pagar"
                Dim displayPagar As Boolean = Convert.ToBoolean(ConfigurationManager.AppSettings("displayPagar"))
                CType(GridView1.Columns.Cast(Of DataControlField)().Where(Function(fld) fld.HeaderText = "Pagar") _
                .SingleOrDefault(), DataControlField).Visible = displayPagar

            Else
                SelectedPreferenceId = String.Empty
                If login.ValidateSessionCookie() = True Then
                Else
                    Response.Redirect("~/login.aspx")
                End If

            End If
        Else
            Response.Redirect("~/login.aspx")
        End If

    End Sub


#Region "Eventos"
    Protected Sub lstConexiones_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstConexiones.SelectedIndexChanged
        Me.divError.Visible = False
        Dim intSocio As Int32
        Dim intConexion As Int32
        If lstConexiones.SelectedValue <> "0" Then
            intSocio = Convert.ToInt32(lstConexiones.SelectedItem.Text.Substring(0, 6))
            intConexion = Convert.ToInt32(lstConexiones.SelectedItem.Text.Substring(7, 4))
        Else
            intSocio = 0
            intConexion = 0
        End If

        readFacturas(intSocio, intConexion)
        Me.GridView1.Caption = "Facturas Conexión " & lstConexiones.SelectedItem.ToString
        If Session("dtsocio") Is Nothing Then
            RecuperarDtSociosYSaldos(Request.Cookies("filtroConexiones").Value)
        End If
        Dim dv As DataView = New DataView(Session("dtsocio"))
        Me.txtNombre.Text = ""
        Me.txtDireccion.Text = ""
        Me.txtLocalidad.Text = ""
        For Each row As DataRowView In dv
            If row("SOCIO") = intSocio And row("CONEXION") = intConexion Then
                Me.txtNombre.Text = row("NOMBRE")
                Me.txtDireccion.Text = row("DIRECION")
                Me.txtLocalidad.Text = row("LOCALIDAD")
                'Me.txtDeudatotal.Text = Format((sumtotal), "0.00")
                Exit For
            End If
        Next
        lstConexiones.Focus()
    End Sub
    Protected Sub cmdPDF_Click(sender As Object, e As ImageClickEventArgs)

    End Sub
    Protected Sub cmdPDF2_Click(sender As Object, e As ImageClickEventArgs)

    End Sub

    Protected Sub gridview1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound

        If e.Row.RowType <> DataControlRowType.Footer And GridView1.Rows.Count > 0 Then

            e.Row.Cells(9).HorizontalAlign = HorizontalAlign.Right
            e.Row.Cells(10).Visible = False
            e.Row.Cells(11).Visible = False
            e.Row.Cells(13).Visible = False
        End If

        If e.Row.RowType = DataControlRowType.Header Then
            e.Row.Cells(0).Text = "Imprime"
            e.Row.Cells(1).Text = "Pagar"
            e.Row.Cells(2).Text = "Factura"
            e.Row.Cells(3).Text = "Pto Venta"
            e.Row.Cells(4).Text = "Socio/Con"
            e.Row.Cells(5).Text = "Período"
            e.Row.Cells(6).Text = "Fecha Emisión"
            e.Row.Cells(7).Text = "Fecha Vto"
            e.Row.Cells(8).Text = "Grupo"
            e.Row.Cells(9).Text = "Importe"
            e.Row.Cells(10).ForeColor = Drawing.Color.Transparent
            e.Row.Cells(10).Visible = False
            e.Row.Cells(11).Visible = False
            e.Row.Cells(12).Text = "Info Pago"
            e.Row.Cells(13).Visible = False

        End If
        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Cells(9).CssClass = "badge bg-red money"
            e.Row.Cells(10).ForeColor = Drawing.Color.Transparent
            e.Row.Cells(10).Visible = False
            e.Row.Cells(11).Visible = False
            e.Row.Cells(13).Visible = False
            'Crear 1 preferencia de pago por fila:

        End If
    End Sub
    Protected Sub gridview1_rowcommand(sender As Object, e As CommandEventArgs) Handles GridView1.RowCommand
        If (e.CommandName = "cmdPDF2") Then
            Dim index1 As Integer = Convert.ToInt32(e.CommandArgument) 'captura el valor del índice de la fila
            Dim row1 As GridViewRow = GridView1.Rows(index1) 'crea un objeto row que contiene la fila del botón presionado.
            Dim socio As Int32 = Convert.ToInt32(CType(row1.Cells(13), DataControlFieldCell).Text)
            Dim numfact As String = socio.ToString.PadLeft(6, "0") &
                    CType(row1.Cells(10), DataControlFieldCell).Text.PadLeft(4, "0") &
                        CType(row1.Cells(2), DataControlFieldCell).Text

            Dim filePDF As String = "~/PDF/" & LTrim(RTrim(numfact)) & ".pdf"
            Dim filePDF2 As String = "PDF/" & LTrim(RTrim(numfact)) & ".pdf"
            Dim retrieveFromFtp As Boolean = Convert.ToBoolean(ConfigurationManager.AppSettings("retrievePdfFromFTP").ToString())
            Try
                Dim myrequest As New WebClient()
                ' Establecer Credenciales para acceder a la carpeta
                myrequest.Credentials = New NetworkCredential(System.Configuration.ConfigurationManager.AppSettings("ftpUser").ToString(), System.Configuration.ConfigurationManager.AppSettings("ftpPassWord").ToString())
                'Read the file data into a Byte array

                Dim myAddress As String = ""
                If System.Configuration.ConfigurationManager.AppSettings("ftpAddress").ToString().Length < 1 Then
                    myAddress = Server.MapPath("/PDF/")
                Else
                    myAddress = System.Configuration.ConfigurationManager.AppSettings("ftpAddress").ToString()
                End If
                If (Not System.IO.Directory.Exists(Server.MapPath("~/_tmp"))) Then
                    System.IO.Directory.CreateDirectory(Server.MapPath("~/_tmp"))
                End If

#Region "Prueba Ricardo FtpWebRequest, funciona OK localmente, igual que el caso siguiente."
                'PRUEBA RICARDO
                'Dim _request As System.Net.FtpWebRequest = System.Net.WebRequest.Create(myAddress)
                If retrieveFromFtp = True Then
                    Dim _request As Net.FtpWebRequest = Net.FtpWebRequest.Create(myAddress & LTrim(RTrim(numfact)) & ".pdf")
                    _request.KeepAlive = False
                    _request.UsePassive = True
                    _request.Timeout = 6000000
                    _request.Method = System.Net.WebRequestMethods.Ftp.DownloadFile
                    _request.Credentials = New NetworkCredential(System.Configuration.ConfigurationManager.AppSettings("ftpUser").ToString(), System.Configuration.ConfigurationManager.AppSettings("ftpPassWord").ToString())
                    Dim _response As System.Net.FtpWebResponse = _request.GetResponse()
                    Using fs As FileStream = File.Create(HttpContext.Current.Server.MapPath("/_tmp/") & LTrim(RTrim(numfact)) & ".pdf")
                        _response.GetResponseStream.CopyTo(fs)
                    End Using
                Else
                    Dim targetPath As String = Server.MapPath("~/_tmp/" & LTrim(RTrim(numfact)) & ".pdf")
                    Dim fileName As String = System.IO.Path.GetFileName(Server.MapPath("/PDF/") & LTrim(RTrim(numfact)) & ".pdf")

                    File.Copy(Server.MapPath("~/PDF/" & fileName), targetPath, True)
                End If

                'Dim responseStream As System.IO.Stream = _response.GetResponseStream().CopyTo(File.Create())

                'Dim bytes() As Byte = myrequest.DownloadData(myAddress & LTrim(RTrim(numfact)) & ".pdf")
                'Dim DownloadStream As FileStream = System.IO.File.Create(HttpContext.Current.Server.MapPath("~/_tmp/") & LTrim(RTrim(numfact)) & ".pdf")
                'DownloadStream.Write(bytes, 0, bytes.Length)
                'DownloadStream.Close()
                ''Dim fs As New System.IO.FileStream(HttpContext.Current.Server.MapPath("~/_tmp/") & LTrim(RTrim(numfact)) & ".pdf", System.IO.FileMode.Create)
                ''responseStream.CopyTo(fs)
                ''responseStream.Close()
                '_response.Close()
                'FIN PRUEBA RICARDO
#End Region
#Region "Descomentar, funcionaba OK, no era por esto"
                'INICIO DESCOMENTAR
                'Dim bytes() As Byte = myrequest.DownloadData(myAddress & LTrim(RTrim(numfact)) & ".pdf")
                '  Crear FileStream para leer el archivo
                'Dim DownloadStream As FileStream = System.IO.File.Create(HttpContext.Current.Server.MapPath("~/_tmp/") & LTrim(RTrim(numfact)) & ".pdf")
                '  Stream al archivo
                'DownloadStream.Write(bytes, 0, bytes.Length)
                'DownloadStream.Close()
                'FIN DESCOMENTAR
#End Region
#Region "Prueba WebRequest"
                'Dim objResponse As WebResponse
                'Dim objRequest As WebRequest
                'Dim result As String
                'objRequest = System.Net.HttpWebRequest.Create("http://www.capsunchales.com/PDF/" & LTrim(RTrim(numfact)) & ".pdf")
                'objRequest.Credentials = New NetworkCredential(System.Configuration.ConfigurationManager.AppSettings("ftpUser").ToString(), System.Configuration.ConfigurationManager.AppSettings("ftpPassWord").ToString())
                'objResponse = objRequest.GetResponse()
                'Dim remoteStream As Stream = Nothing
                'Dim localStream As Stream = Nothing
                'localStream = File.Create(Server.MapPath("~/_tmp/" & LTrim(RTrim(numfact)) & ".pdf"))
                'Dim sr As New StreamReader(objResponse.GetResponseStream())
                'Dim stream As Stream = objResponse.GetResponseStream()
                'Dim buffer As Byte() = New Byte(1023) {}
                'Dim bg1 As Int64 = buffer.Length
                'Dim bytesread As Byte = stream.Read(buffer, 0, bg1)
                'localStream.Write(buffer, 0, bytesread)
                ''result = sr.ReadToEnd()
                ''Dim mst As MemoryStream = sr.ReadToEnd().ToArray
                ''HttpContext.Current.Response.ClearContent()
                ''HttpContext.Current.Response.ClearHeaders()
                ''HttpContext.Current.Response.ContentType = "application/pdf"
                ''HttpContext.Current.Response.OutputStream.Write(mst.ToArray(), 0, mst.ToArray().Length)
                ''HttpContext.Current.Response.Flush()
                ''HttpContext.Current.Response.Close()

                'sr.Close()

                '' Create a request for the URL. 
                'Dim request As WebRequest = WebRequest.Create("http://www.capsunchales.com/PDF/" & LTrim(RTrim(numfact)) & ".pdf")
                '' If required by the server, set the credentials.
                'request.Credentials = New NetworkCredential(System.Configuration.ConfigurationManager.AppSettings("ftpUser").ToString(), System.Configuration.ConfigurationManager.AppSettings("ftpPassWord").ToString())
                '' Get the response.
                'Dim response As WebResponse = request.GetResponse()
                'Dim outFile As String = "c:\google.pdf"
                '' Get the stream containing content returned by the server.
                'Dim dataStream As Stream = response.GetResponseStream()
                '' Open the stream using a StreamReader for easy access.
                'Dim reader As New StreamReader(dataStream)
                '' Read the content.
                'Dim responseFromServer As String = reader.ReadToEnd()
                '' Display the content.
                'Console.WriteLine(responseFromServer)
                '' Clean up the streams and the response.
                'reader.Close()
                'response.Close()
#End Region

                Response.Cookies.Remove("fileName")
                Dim pdfCookie As HttpCookie
                pdfCookie = New HttpCookie("filename")
                pdfCookie.Value = LTrim(RTrim(numfact)) & ".pdf"
                pdfCookie.Expires = DateTime.Now.AddMinutes(10)
                Response.Cookies.Add(pdfCookie)

                Response.Redirect("~/showPdf.aspx")
            Catch ex As Exception
                Me.lblError.Text = "No se pudo recuperar el documento PDF."
                Me.divError.Visible = True
            End Try
        Else
            If (e.CommandName.Equals("btnPagar")) Then
                Dim index1 As Integer = Convert.ToInt32(e.CommandArgument) 'captura el valor del índice de la fila
                Dim row1 As GridViewRow = GridView1.Rows(index1) 'crea un objeto row que contiene la fila del botón presionado.
                Dim socio As Int32 = Convert.ToInt32(CType(row1.Cells(13), DataControlFieldCell).Text)
                Dim numfact As String = CType(row1.Cells(2), DataControlFieldCell).Text
                Dim importe As String = CType(row1.Cells(9), DataControlFieldCell).Text
                importeFactura = ConvertirCampoImporteADecimal(importe)
                Dim Master As Trylogyc = CType(Me.Master, Trylogyc)
                Dim idSocio As String = socio
                Dim idConexion As String = CType(row1.Cells(10), DataControlFieldCell).Text
                Dim pagoEnProceso As Boolean = VerificarPagoEnProceso(idSocio, idConexion, numfact, importeFactura)
                If pagoEnProceso = True Then
                    Response.Redirect(String.Format("~/ConfirmarPago.aspx?numfact={0}&importe={1}&idSocio={2}&idConexion={3}&IDUsuario={4}", LTrim(RTrim(numfact)), importe, idSocio, idConexion, Session("IDUsuario")))
                Else
                    Response.Redirect(String.Format("~/Pagar.aspx?numfact={0}&importe={1}&idSocio={2}&idConexion={3}&IDUsuario={4}", LTrim(RTrim(numfact)), importe, idSocio, idConexion, Session("IDUsuario")))
                End If
            End If
        End If


    End Sub
    Protected Sub GridView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GridView1.SelectedIndexChanged

    End Sub
#End Region

#Region "Métodos Privados"

    Private Sub readFacturas(ByVal xmlsocio As String, ByVal conexion As Int32)
        Try
            If Session("dtSaldo") Is Nothing Then
                RecuperarDtSociosYSaldos(Request.Cookies("filtroConexiones").Value)
            End If
            Dim dvFacturas As New DataView(Session("dtSaldo"))
            Dim dtFacturas As New DataTable
            Dim dvdtFacturas As New DataTable
            'Dim importeFactura As Decimal

            dtFacturas.Columns.Add("Nro_Factura")
            dtFacturas.Columns.Add("Pto_Venta")
            dtFacturas.Columns.Add("Letra")
            dtFacturas.Columns.Add("Periodo")
            dtFacturas.Columns.Add("Fecha_Emision")
            dtFacturas.Columns.Add("Fecha_Vto")
            dtFacturas.Columns.Add("Grupo_Fact")
            dtFacturas.Columns.Add("Importe")
            dtFacturas.Columns.Add("Factura")
            dtFacturas.Columns.Add("Conexion")
            dtFacturas.Columns.Add("Pagada")
            dtFacturas.Columns.Add("IdPreferenciaPago")
            dtFacturas.Columns.Add("Socio")

            If conexion = 0 Then 'TODAS
                'dvFacturas.RowFilter = "Socio =" & xmlsocio
                dvFacturas.Sort = "PERIODO DESC"
                dvdtFacturas = dvFacturas.ToTable
                For u = 0 To dvdtFacturas.Rows.Count - 1
                    dtFacturas.Rows.Add()
                    dtFacturas.Rows(u).Item("Nro_Factura") = Mid(dvdtFacturas.Rows(u).Item("Factura"), 11, (Len(dvdtFacturas.Rows(u).Item("Factura")) - 10))
                    dtFacturas.Rows(u).Item("Pto_Venta") = dvdtFacturas.Rows(u).Item("Pto_Venta")
                    dtFacturas.Rows(u).Item("Letra") = dvdtFacturas.Rows(u).Item("Socio") & "/" & dvdtFacturas.Rows(u).Item("Conexion")  'dvdtFacturas.Rows(u).Item("Letra")
                    dtFacturas.Rows(u).Item("Periodo") = dvdtFacturas.Rows(u).Item("Periodo")
                    dtFacturas.Rows(u).Item("Fecha_Emision") = dvdtFacturas.Rows(u).Item("Fecha_Emision")
                    dtFacturas.Rows(u).Item("Fecha_Vto") = dvdtFacturas.Rows(u).Item("Fecha_Vto")
                    dtFacturas.Rows(u).Item("Grupo_Fact") = dvdtFacturas.Rows(u).Item("Grupo_Fact")
                    dtFacturas.Rows(u).Item("Importe") = dvdtFacturas.Rows(u).Item("Importe")
                    dtFacturas.Rows(u).Item("Factura") = dvdtFacturas.Rows(u).Item("Factura")
                    dtFacturas.Rows(u).Item("Conexion") = dvdtFacturas.Rows(u).Item("Conexion")
                    dtFacturas.Rows(u).Item("Pagada") = dvdtFacturas.Rows(u).Item("Pagada")
                    dtFacturas.Rows(u).Item("Socio") = dvdtFacturas.Rows(u).Item("Socio")
                    'TODO: Para la fila 10, si la factura está en la tabla nueva a crear "Pagos", mostrar "Pago en proceso" y quizás darle un color.
                    sumtotal = sumtotal + Convert.ToDouble(dvdtFacturas.Rows(u).Item("Importe"))
                Next
                'Me.txtDeudatotal.Text = Format((sumtotal), "0.00")

            Else
                dvFacturas.RowFilter = "Socio =" & xmlsocio & " AND Conexion =" & conexion
                'dvFacturas.RowFilter = "Conexion =" & conexion
                dvFacturas.Sort = "periodo DESC"
                dvdtFacturas = dvFacturas.ToTable
                For u = 0 To dvdtFacturas.Rows.Count - 1
                    dtFacturas.Rows.Add()
                    dtFacturas.Rows(u).Item("Nro_Factura") = Mid(dvdtFacturas.Rows(u).Item("Factura"), 11, (Len(dvdtFacturas.Rows(u).Item("Factura")) - 10))
                    dtFacturas.Rows(u).Item("Pto_Venta") = dvdtFacturas.Rows(u).Item("Pto_Venta")
                    dtFacturas.Rows(u).Item("Letra") = dvdtFacturas.Rows(u).Item("Socio") & "/" & dvdtFacturas.Rows(u).Item("Conexion") 'dvdtFacturas.Rows(u).Item("Letra")
                    dtFacturas.Rows(u).Item("Periodo") = dvdtFacturas.Rows(u).Item("Periodo")
                    dtFacturas.Rows(u).Item("Fecha_Emision") = dvdtFacturas.Rows(u).Item("Fecha_Emision")
                    dtFacturas.Rows(u).Item("Fecha_Vto") = dvdtFacturas.Rows(u).Item("Fecha_Vto")
                    dtFacturas.Rows(u).Item("Grupo_Fact") = dvdtFacturas.Rows(u).Item("Grupo_Fact")
                    dtFacturas.Rows(u).Item("Importe") = dvdtFacturas.Rows(u).Item("Importe")
                    dtFacturas.Rows(u).Item("Factura") = dvdtFacturas.Rows(u).Item("Factura")
                    dtFacturas.Rows(u).Item("Conexion") = dvdtFacturas.Rows(u).Item("Conexion")
                    dtFacturas.Rows(u).Item("Pagada") = dvdtFacturas.Rows(u).Item("Pagada")
                    dtFacturas.Rows(u).Item("Socio") = dvdtFacturas.Rows(u).Item("Socio")
                    'TODO: Para la fila 10, si la factura está en la tabla nueva a crear "Pagos", mostrar "Pago en proceso" y quizás darle un color.
                    sumtotal = sumtotal + Convert.ToDouble(dvdtFacturas.Rows(u).Item("Importe"))
                Next


            End If

            Me.GridView1.DataSource = ""
            Me.GridView1.DataSource = dtFacturas
            Me.GridView1.DataBind()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub ReEvaluarYSetearMercadoPagoToken()
        If (MercadoPago.SDK.AccessToken Is Nothing) Then
            MercadoPago.SDK.SetAccessToken("TEST-803559796931547-060612-07b906157e8808e7d657beabf35fdc1d-229476782")
        End If
    End Sub
    Private Function GenerarPreferenciaPagoMercadoPago(nroFactura As String, importeFactura As Decimal) As String
        'Dim importeAPagar As Decimal
        Dim myPreference As New Preference()
        myPreference.ClientId = "580430370"
        Dim itemToAdd As New Item()
        itemToAdd.Title = nroFactura
        itemToAdd.Quantity = 1
        itemToAdd.CurrencyId = CurrencyId.ARS
        itemToAdd.UnitPrice = importeFactura
        myPreference.Items.Add(itemToAdd)
        Dim _backUrls As New BackUrls()
        _backUrls.Success = Request.Url.GetLeftPart(UriPartial.Path) & "/PagoExitoso.aspx"
        _backUrls.Failure = Request.Url.GetLeftPart(UriPartial.Path) & "/PagoRechazado.aspx"
        _backUrls.Pending = Request.Url.GetLeftPart(UriPartial.Path) & "/PagoPendiente.aspx"
        myPreference.BackUrls = _backUrls
        myPreference.AutoReturn = AutoReturnType.approved
        myPreference.Save()
        GenerarPreferenciaPagoMercadoPago = myPreference.Id
        Return myPreference.Id
    End Function
    Private Function ConvertirCampoImporteADecimal(ByVal cell As Object) As Decimal
        Dim importeFactura As Decimal

        If cell Is Nothing Or cell Is DBNull.Value Then
            Session("txtError") = "Se produjo un error. Los montos de las facturas no tienen el formato correcto."
            Response.Redirect("~/Error.aspx")
        Else
            If (Decimal.TryParse(cell,
            NumberStyles.AllowDecimalPoint,
            CultureInfo.CreateSpecificCulture("fr-FR"),
            importeFactura) = False) Then
                Session("txtError") = "Se produjo un error. Los montos de las facturas no tienen el formato correcto."
                Response.Redirect("~/Error.aspx")
            End If
        End If

        ConvertirCampoImporteADecimal = importeFactura
        Return importeFactura
    End Function

    Private Function VerificarPagoEnProceso(ByVal idSocio As Int32, ByVal idConexion As Int32, ByVal numFact As String, ByVal importe As Decimal) As Boolean
        Dim myContext As New TrylogycContext
        Dim existePago As Boolean = myContext.GetPago(idSocio, idConexion, numFact, importe)
        Return existePago
    End Function

    Private Sub CargarComboConexiones(ByVal dtSocio As DataTable)

        For r = 0 To dtSocio.Rows.Count - 1
            Dim intSocio As Int32 = Convert.ToInt32(dtSocio.Rows(r).Item("Socio"))
            Dim strSocio As String = intSocio.ToString().PadLeft(6, "0")
            Dim miItem As New ListItem
            Dim itConexion = dtSocio.Rows(r).Item("Conexion")
            Dim itcnt As Int32
            Dim strConexion As String = ""
            strConexion = itConexion.ToString.PadLeft(4, "0")
            itcnt = lstConexiones.Items.Count
            miItem.Value = strSocio & "-" & strConexion
            miItem.Text = strSocio & "-" & strConexion
            lstConexiones.Items.Insert(r + 1, miItem)
            'lstConexiones.Items.Item(r + 1).Value = strConexion
            'lstConexiones.Items.Item(r + 1).Text = strSocio & "-" & strConexion
        Next r
        lstConexiones.SelectedValue = 0
    End Sub

    Private Function RecuperarDtSociosYSaldos(ByVal filtroConexiones As String) As DataTable
        Dim xmlFile2 As XmlReader = XmlReader.Create(Server.MapPath("/xmlfiles/SALDOS.xml"), New XmlReaderSettings())
        Dim ds2 As New DataSet()
        ds2.ReadXml(xmlFile2)
        Dim dtSaldos As DataTable = ds2.Tables(0)
        Dim dvSaldos As New DataView(dtSaldos)
        dvSaldos.RowFilter = filtroConexiones
        'dvSaldos.RowFilter = "Socio = " & Session("xmlSocio") & " AND (" & filtroConexiones & ")"
        Session("dtSaldo") = dvSaldos.ToTable
        Dim ds As New DataSet()
        Dim xmlFile = XmlReader.Create(Server.MapPath("/xmlfiles/SOCIOS.xml"), New XmlReaderSettings())
        ds.ReadXml(xmlFile)
        'ds2.ReadXml(xmlFile2)
        Dim dtSocios As DataTable = ds.Tables(0)
        Dim dvSocios As New DataView(dtSocios)
        dvSocios.RowFilter = filtroConexiones
        'dvSocios.RowFilter = "SOCIO = '" & Session("xmlSocio") & "' AND (" & filtroConexiones & ")"
        Dim conCount As Int32 = dvSocios.Count
        Session("conCount") = conCount
        Session("dtSocio") = dvSocios.ToTable
    End Function

#End Region
End Class