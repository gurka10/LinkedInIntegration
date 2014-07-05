<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LinkedInIntegration._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div>
        <fieldset>

            <h3>Connect to your profile</h3>
            
            <div id="connectionId" style="border: solid 1px #ccc; background-color: #eee;" runat="server">
                <table>

                    <tr>
                        <td>Click to comnnect to LinkedIn :
                        </td>
                        <td>
                            <asp:HyperLink ID="hypAuthToken" runat="server"></asp:HyperLink>
                        </td>
                    </tr>
                </table>
            </div>

            <br />
            <div style="border: solid 1px #ccc; background-color: #eee;">
                <h3>Your Details</h3>
                <asp:TextBox ID="lblYourDetails" ReadOnly="true" runat="server" TextMode="MultiLine" Rows="10" Width="650px"></asp:TextBox>
            </div>
            <asp:Button ID="btnPersist" runat="server" Text="Find User By ProfileId" Visible="false" OnClick="HomeBtn_Click" />
        </fieldset>
    </div>
</asp:Content>