using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Text;
using System.Text.Json;
var email = new MimeMessage();
email.From.Add(MailboxAddress.Parse("wjaco@uaisoft.com.br"));
email.To.Add(MailboxAddress.Parse("wagnerjaco@gmail.com"));
email.Subject = "Produtos com baixo estoque";
var produtos = (using using Elsa.Server.Web.DB.Class.Produto[])Variables.Get("Lista");
// Monta tabela HTML
var itensHtml = new StringBuilder();
foreach (var item in produtos)
{
    itensHtml.Append($@"
        <tr>
            <td>{item.Id}</td>
            <td>{item.Nome}</td>
            <td>{item.EstoqueAtual}</td>
            <td>{item.EstoqueMinimo}</td>
        </tr>
    ");
}
// HTML do email
var html = $@"
<html>
<head>
<style>
body {{
    font-family: Arial;
    background-color:#f5f5f5;
    padding:20px;
}}

.container {{
    background:white;
    padding:30px;
    border-radius:10px;
}}

table {{
    width:100%;
    border-collapse: collapse;
}}

th, td {{
    border:1px solid #ddd;
    padding:10px;
    text-align:left;
}}

th {{
    background:#2563eb;
    color:white;
}}
</style>
</head>
<body>
<div class='container'>
<h2>Produtos com baixo estoque</h2>
<p>
Os seguintes produtos estão abaixo do estoque mínimo:
</p>
<table>
    <thead>
        <tr>
            <th>ID</th>
            <th>Produto</th>
            <th>Estoque Atual</th>
            <th>Estoque Mínimo</th>
        </tr>
    </thead>

    <tbody>
        {itensHtml}
    </tbody>
</table>
</div>
</body>
</html>
";
email.Body = new TextPart("html") { Text = html };
var smtp = new SmtpClient();
smtp.Connect("smtp.skymail.net.br", 587, SecureSocketOptions.StartTls);
smtp.Authenticate("wjaco@uaisoft.com.br", "291013@W.a");
string status = "";
try
{
    smtp.Send(email);
    status = "sucesso";
}
catch (Exception ex)
{ status = ex.Message; }
smtp.Disconnect(true);
return status;