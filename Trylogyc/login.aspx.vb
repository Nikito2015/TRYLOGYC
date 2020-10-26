Imports Trylogyc.Models
Imports Trylogyc.DAL
Imports System.IO
Imports System.Xml
Imports System.Net
Imports System.Reflection

Public Class login
    Inherits System.Web.UI.Page

#Region "Properties"
    Private _IDUsuario As Int32
    Public Property IDUsuario() As Int32
        Get
            Return _IDUsuario
        End Get
        Set(ByVal value As Int32)
            _IDUsuario = value
        End Set
    End Property

#End Region
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'login.ValidateSessionCookie() = False
        HttpContext.Current.Request.Cookies.Remove("SessionId")
    End Sub

    Protected Sub btnlogin_Click(sender As Object, e As EventArgs) Handles btnlogin.Click

        '1.Instanciar Servicio
        Dim proxy As New WcfTrylogycWebsite.ServiceClient()
        '2.Crear clase Request.
        Dim loginRequest As New WcfTrylogycWebsite.LoginRequest()

        loginRequest.Password = Me.txtpassword.Text
        loginRequest.Email = Me.txtemail.Text

        '3.Invocar Servicio
        Dim loginResponse As WcfTrylogycWebsite.LoginResponse = proxy.Login(loginRequest)

        If loginResponse.StatusCode = HttpStatusCode.OK Then
            CreateSessionCookie()
            'Session("IDUsuario") = loginResponse.User.Id
            IDUsuario = loginResponse.User.Id
            'Session("xmlSocio") = loginResponse.User.Associates.FirstOrDefault().Id
            Me.Context.Items("xmlSocio") = loginResponse.User.Associates.FirstOrDefault().Id
            'Session("nomUsuario") = loginResponse.User.UserName
            Me.Context.Items("nomUsuario") = loginResponse.User.UserName
            'Session("userEmail") = loginResponse.User.Email
            Me.Context.Items("userEmail") = loginResponse.User.Email
            'Session("aceptaFacturaMail") = loginResponse.User.EmailInvoice
            Me.Context.Items("aceptaFacturaMail") = loginResponse.User.EmailInvoice
            'Session("conCount") = loginResponse.User.TotalConnections
            Me.Context.Items("conCount") = loginResponse.User.TotalConnections
            'Session("dtSocio") = CreateDataTableAssociates(loginResponse.User.Associates)
            Me.Context.Items("dtSocio") = CreateDataTableAssociates(loginResponse.User.Associates)
            'Session("dtSaldo") = CreateDataTableBalances(loginResponse.User.Balances)
            Me.Context.Items("dtSaldo") = CreateDataTableBalances(loginResponse.User.Balances)
            'Session("Foto") = loginResponse.User.Picture
            Me.Context.Items("Foto") = loginResponse.User.Picture
            'Session("Password") = loginResponse.User.Password
            Me.Context.Items("Password") = loginResponse.User.Password
            'Session("Token") = loginResponse.Token
            Me.Context.Items("Token") = loginResponse.Token
            '(CType(Me.Master, Trylogyc)).strName
            Server.Transfer(loginResponse.User.Route, True)
            'Response.Redirect(loginResponse.User.Route)
        Else
            lblError.Text = loginResponse.Message
            lblError.Visible = True
        End If

    End Sub

#Region "Metodos privados"

    ''' <summary>
    ''' Creates the data table associates.
    ''' </summary>
    ''' <param name="associates">The associates.</param>
    ''' <returns></returns>
    Private Function CreateDataTableAssociates(ByVal associates As WcfTrylogycWebsite.WcfAssociate()) As DataTable
        Dim associatesList = associates.[Select](Function(elem) New With {
        Key .Socio = elem.Id,
        Key .Conexion = elem.ConnectionId,
        Key .CGP = elem.CGP,
        Key .Nombre = elem.Name,
        Key .Direccion = elem.Address,
        Key .Localidad = elem.City,
        Key .Documento = elem.Document
    })

        Dim dtAssociates As New DataTable()
        dtAssociates.Columns.Add("Socio")
        dtAssociates.Columns.Add("Conexion")
        dtAssociates.Columns.Add("CGP")
        dtAssociates.Columns.Add("Nombre")
        dtAssociates.Columns.Add("Direcion")
        dtAssociates.Columns.Add("Localidad")
        dtAssociates.Columns.Add("Documento")

        For Each associate In associatesList
            dtAssociates.Rows.Add(New Object() {associate.Socio, associate.Conexion, associate.CGP, associate.Nombre, associate.Direccion, associate.Localidad, associate.Documento})
        Next
        CreateDataTableAssociates = dtAssociates

    End Function

    ''' <summary>
    ''' Creates the data table balances.
    ''' </summary>
    ''' <param name="balances">The balances.</param>
    ''' <returns></returns>
    Private Function CreateDataTableBalances(ByVal balances As WcfTrylogycWebsite.WcfBalance()) As DataTable
        Dim balancesList = balances.[Select](Function(elem) New With {
            Key .Socio = elem.AssociateId,
            Key .Conexion = elem.ConnectionId,
            Key .Periodo = elem.Period,
            Key .Grupo_Fact = elem.InvoiceGroup,
            Key .Letra = elem.InvoiceLetter,
            Key .Pto_Venta = elem.InvoicePoint,
            Key .Numero = elem.InvoiceNumber,
            Key .Fecha_Emision = elem.InvoiceDate,
            Key .Fecha_Vto = elem.InvoiceExpirationDate,
            Key .Importe = elem.InvoiceAmmount,
            Key .Factura = elem.InvoiceTrackingNumber,
            Key .Pagada = elem.Paid
        })

        Dim dtBalances As New DataTable()
        dtBalances.Columns.Add("Socio")
        dtBalances.Columns.Add("Conexion")
        dtBalances.Columns.Add("Periodo")
        dtBalances.Columns.Add("Grupo_Fact")
        dtBalances.Columns.Add("Letra")
        dtBalances.Columns.Add("Pto_Venta")
        dtBalances.Columns.Add("Numero")
        dtBalances.Columns.Add("Fecha_Emision")
        dtBalances.Columns.Add("Fecha_Vto")
        dtBalances.Columns.Add("Importe")
        dtBalances.Columns.Add("Factura")
        dtBalances.Columns.Add("Pagada")


        For Each balance In balancesList
            dtBalances.Rows.Add(New Object() {
            balance.Socio, balance.Conexion, balance.Periodo, balance.Grupo_Fact, balance.Letra, balance.Pto_Venta, balance.Numero, balance.Fecha_Emision,
            balance.Fecha_Vto, balance.Importe, balance.Factura, balance.Pagada
            })
        Next
        CreateDataTableBalances = dtBalances
    End Function

    ''' <summary>
    ''' Creates the session cookie.
    ''' </summary>
    Private Sub CreateSessionCookie()
        Dim sessionTimeOut As Int32 = Convert.ToInt32(ConfigurationManager.AppSettings("SessionTimeOut").ToString())
        Dim cookie As HttpCookie
        cookie = New HttpCookie("SessionId")
        cookie.Value = Guid.NewGuid().ToString()
        cookie.Expires = DateTime.Now.AddMinutes(sessionTimeOut)
        HttpContext.Current.Response.Cookies.Add(cookie)
    End Sub

    ''' <summary>
    ''' Validates the session cookie.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function ValidateSessionCookie() As Boolean
        Dim sessionTimeOut As Int32 = Convert.ToInt32(ConfigurationManager.AppSettings("SessionTimeOut").ToString())
        Dim cookie As HttpCookie
        cookie = HttpContext.Current.Request.Cookies("SessionId")
        If cookie Is Nothing Then
            ValidateSessionCookie = False
        Else
            HttpContext.Current.Response.Cookies.Item("SessionId").Expires = DateTime.Now.AddMinutes(sessionTimeOut)
            ValidateSessionCookie = True
        End If
    End Function
#End Region
End Class