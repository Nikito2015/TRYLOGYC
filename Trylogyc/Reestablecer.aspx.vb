Imports Trylogyc.DAL
Imports System.Net.Mail
Public Class Reestablecer
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnRegister_Click(sender As Object, e As EventArgs) Handles btnRegister.Click
        Dim myContext As New TrylogycContext
        Dim email As String = Request.Form("email")
        Dim codigo = Request.Form("codigo")
        Dim codigo1 = Mid(codigo, 1, 6)
        Dim intCodigo As Int32
        If Int32.TryParse(codigo1, intCodigo) = True And email <> "" Then
            Dim checkSocio = myContext.GetUsuarioReestablecer(intCodigo, email)
            If checkSocio IsNot Nothing And checkSocio.Tables.Count > 0 Then
                If checkSocio.Tables(0).Rows.Count > 0 Then 'Significa que existe un socio/usuario con dicho código
                    Dim userPass = checkSocio.Tables(0).Rows(0).Item(0)
                    Dim userEmail = checkSocio.Tables(0).Rows(0).Item(1)
                    send_reg_mail(userEmail, userPass)

                Else
                    lblError.Text = "No existe un Socio con ese email"
                    lblError.Visible = True
                End If
            Else
                lblError.Text = "No existe un Socio con ese email"
                lblError.Visible = True
            End If
        Else
            lblError.Text = "Datos erróneos"
            lblError.Visible = True
        End If

    End Sub

    Private Sub send_reg_mail(ByVal email As String, password As String)
        Dim a = System.Configuration.ConfigurationManager.AppSettings("email").ToString()
        If Len(a) > 7 Then
            Try


                Dim b = System.Configuration.ConfigurationManager.AppSettings("password").ToString()
                Dim c = System.Configuration.ConfigurationManager.AppSettings("site").ToString()
                Dim d = System.Configuration.ConfigurationManager.AppSettings("host").ToString()
                Dim e = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings("port").ToString())
                Dim f = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings("usedefaultcredentials").ToString())
                Dim g = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings("enablessl").ToString())
                With sqlexito
                    Dim j As Int32 = 0
                    For j = 0 To 1
                        If j < 1 Then

                            Dim message = New MailMessage()
                            message.From = New MailAddress(System.Configuration.ConfigurationManager.AppSettings("email").ToString(), System.Configuration.ConfigurationManager.AppSettings("site").ToString())
                            message.To.Add(New MailAddress(email))
                            message.Subject = "Sus datos de Acceso al sitio" & System.Configuration.ConfigurationManager.AppSettings("site").ToString()
                            message.IsBodyHtml = True
                            message.Body = "<html><body><span style='font-family:Georgia;font-size:14px;font-style:normal;font-weight:normal;text-decoration:none;text-transform:none;color:000000;background-color:ffffff;'><p>" &
                                            "<br/>Su contraseña para acceder al sitio: " & password &
                                            "<br/>" &
                                            "</p></span></body></html>"
                            Dim client As New SmtpClient()
                            With client
                                Try
                                    .Host = System.Configuration.ConfigurationManager.AppSettings("host").ToString()
                                    .Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings("port").ToString())
                                    .UseDefaultCredentials = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings("usedefaultcredentials").ToString())
                                    .Credentials = New System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings("email").ToString(),
                                                                                    System.Configuration.ConfigurationManager.AppSettings("password").ToString())
                                    .EnableSsl = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings("enablessl").ToString())
                                    .Send(message)
                                    message.Dispose()
                                Catch ex As Exception
                                End Try
                                Me.lblError.Visible = False
                                Response.Write("<script>alert('Verifique su email para obtener sus datos de acceso.');</script>")

                            End With

                        End If
                    Next

                End With
            Catch ex As Exception
                Response.Write("<script>alert('Han ocurrido errores durante el envío del email.');</script>")
            End Try
        Else
            Response.Write("<script>alert('Han ocurrido errores durante el envío del email.');</script>")
        End If
    End Sub

End Class