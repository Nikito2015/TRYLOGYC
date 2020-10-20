Imports Trylogyc.DAL
Imports System.Net.Mail

Public Class register
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Me.Form.DefaultButton = Me.btnRegister.UniqueID
        End If
    End Sub

    Protected Sub btnRegister_Click(sender As Object, e As EventArgs) Handles btnRegister.Click
        Dim myContext As New TrylogycContext
        Dim email = Request.Form("email")
        Dim confEmail = Request.Form("confirmEmail")
        Dim codigo = Request.Form("codigo")
        Dim cgp = Request.Form("cgp")
        Dim mError As String = ""
        If email = confEmail And email <> "" Then

            If Len(codigo) = 10 Then
                Dim IDSocio As Int32
                Dim IDConexion As Int32
                Dim cntSocio As Int32
                Dim aceptaFacturaMail As Boolean = aceptaEmail.Checked
                IDSocio = Convert.ToInt32(Mid(codigo, 1, 6))
                IDConexion = Convert.ToInt32(Mid(codigo, 8, 4))
                'ir a la base y revisar que ese xmlSocio no tenga un usuario ya asignado.
                cntSocio = myContext.GetUsuarioSocio(IDSocio, email)
                If cntSocio > 0 Then 'el socio ya tiene una cuenta creada
                    mError = "Ya existe un usuario registrado con esos datos."
                Else 'no hay usuario registrado para el codigo de socio ingresado
                    Dim checkSocio = myContext.GetSocios() 'validar que el nro. de socio ingresado corresponda a un socio
                    If checkSocio.Tables(0).TableName = "Error" Then
                        Session("codError") = checkSocio.Tables(0).Rows(0).Item(0)
                        Session("txtError") = checkSocio.Tables(0).Rows(0).Item(1)
                        mError = "Error" & checkSocio.Tables(0).Rows(0).Item(0)
                    Else
                        If checkSocio.Tables(0) IsNot Nothing Then
                            For Each r In checkSocio.Tables(0).Rows
                                If Convert.ToInt32(r.Item(0)) = IDSocio And
                                Convert.ToInt32(r.Item(1)) = IDConexion Then
                                    If r.Item(2) = cgp Then 'Para dicho socio y conexion que coincida el CGP
                                        'registrar usuario
                                        Dim dsnewUser As DataSet
                                        dsnewUser = myContext.PutUsuario(email, IDSocio, IDConexion, aceptaFacturaMail)
                                        If dsnewUser IsNot Nothing And dsnewUser.Tables.Count > 0 Then 'se insertó nuevo usuario y me devolvió el password
                                            send_reg_mail(email, dsnewUser.Tables(0).Rows(0).Item(1))
                                            mError = "Usuario Registrado Exitosamente, revise su cuenta de correo electrónico."
                                        End If
                                        Exit For
                                    Else
                                        mError = "El código de Gestión Personal ingresado es incorrecto."
                                        Exit For
                                    End If
                                End If
                            Next
                        Else
                            mError = "Codigo de Cliente Inexistente"
                        End If
                    End If
                End If
            Else
                mError = "Codigo de Cliente debe tener un tamaño de 10 caracteres."
            End If
        Else
            mError = "Los e-Mail ingresados no coinciden."
        End If
        If mError <> "" Then
            lblError.Text = mError
            lblError.Visible = True
        End If
    End Sub

    Private Sub send_reg_mail(ByVal email As String, codigo As String)
        If Len(email) > 7 Then
            Try
                With sqlexito
                    Dim j As Int32 = 0
                    For j = 0 To 1
                        If j < 1 Then

                            Dim message = New MailMessage()
                            message.From = New MailAddress(System.Configuration.ConfigurationManager.AppSettings("email").ToString(), System.Configuration.ConfigurationManager.AppSettings("site").ToString())
                            message.To.Add(New MailAddress(email))
                            message.Subject = "Sus datos de Acceso al sitio " & System.Configuration.ConfigurationManager.AppSettings("site").ToString()
                            message.IsBodyHtml = True
                            message.Body = "<html><body><span style='font-family:Georgia;font-size:14px;font-style:normal;font-weight:normal;text-decoration:none;text-transform:none;color:000000;background-color:ffffff;'><p>" &
                            "Nueva Consulta desde Mi sitio Web" &
                                            "<br/>Su contraseña para acceder al sitio: " & codigo &
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
                                    Throw ex
                                End Try
                                Me.lblError.Visible = False
                                Response.Write("<script>alert('Verifique su email para obtener sus datos de acceso.');</script>")
                                'Response.Redirect("~/login.aspx")
                            End With

                        End If
                    Next

                End With
            Catch ex As Exception
                Me.lblError.Visible = True
                Me.lblError.Text = "Su usuario se registró exitosamente pero no pudimos enviar el mail de registro."

            End Try
        End If
    End Sub


End Class