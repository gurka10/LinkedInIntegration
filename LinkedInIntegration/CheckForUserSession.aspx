<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckForUserSession.aspx.cs" Inherits="LinkedInIntegration.CheckForUserSession" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <p>
                If you are already logged in then will persist your member details:-
                <br />
                <asp:TextBox ID="lblYourDetails" ReadOnly="true" runat="server" TextMode="MultiLine" Rows="10" Width="650px"></asp:TextBox>

                <br /><br />
                <br /><br />
                <b>Enter your public profile url below:-</b>
                <asp:TextBox ID="txtProfileUrl" runat="server"></asp:TextBox>
                <br />
                <asp:Button ID="bntSearch" runat="server" Text="Search" Visible="true" OnClick="SearchBtn_Click" />
                <br /><br />
                <asp:Button ID="btnReturn" runat="server" Text="Back Home"  Visible="true" OnClick="HomeBtn_Click" />
            </p>
        </div>
    </form>
</body>
</html>