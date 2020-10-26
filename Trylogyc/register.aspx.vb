Imports Trylogyc.DAL
Imports System.Net.Mail
Imports System.Net

Public Class register
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Me.Form.DefaultButton = Me.btnRegister.UniqueID
        End If
    End Sub

    Protected Sub btnRegister_Click(sender As Object, e As EventArgs) Handles btnRegister.Click
        'Dim myContext As New TrylogycContext       
        Dim email = Request.Form("email")
        Dim confEmail = Request.Form("confirmEmail")
        Dim codigo = Request.Form("codigo")
        Dim cgp = Request.Form("cgp")

        '1.Instanciar Servicio
        Dim proxy As New WcfTrylogycWebsite.ServiceClient()
        '2.Crear clase Request.
        Dim registerRequest As New WcfTrylogycWebsite.RegisterRequest()

        registerRequest.Email = email
        registerRequest.EmailConfirm = confEmail
        registerRequest.Code = codigo
        registerRequest.CGP = cgp
        registerRequest.EmailInvoices = Convert.ToBoolean(aceptaEmail.Checked)

        '3.Invocar Servicio
        Dim register As WcfTrylogycWebsite.RegisterResponse = proxy.Register(registerRequest)
        lblError.Text = register.Message
        lblError.Visible = True

    End Sub

End Class