using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Silent_Print {
    class Helper {
        /// <summary>
        /// Class <c>getBetween</c> get string between two string
        /// </summary>
        private string getBetween(string strSource, string strStart, string strEnd) {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd)) {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            } else {
                return "";
            }
        }

        /// <summary>
        /// Class <c>GetDocInfo</c> get document number and type
        /// </summary>
        public string[] GetDocInfo(string url) {
            Uri myUri = new Uri(url);
            string host = myUri.Host;
            string splitt = getBetween(url, "http", ":");
            string local = "";
            if (splitt == "") {
                local = "http://";
            } else if (splitt == "s") {
                local = "https://";
            }

            string loginID = getBetween(url, "&loginID=", "&");
            string companyID = getBetween(url, "&companyID=", "&");
            string documentID = getBetween(url, "?id=", "&");
            string con = getBetween(url, "&con=", "&");
            string mod = getBetween(url, "&mod=", "&");

            string reqURL = local + host + "/print_layout/get_doc_info.php?loginID=" + loginID + "&companyID=" + companyID + "&doc_id=" + documentID + "&menu_module=" + mod + "&menu_controller=" + con;
            string respValue = SendRequest(reqURL, false);
            
            string[] docInfo;
            if (respValue == null || respValue == "") {
                return null;
            } else {
                respValue = Regex.Unescape(respValue).Trim('"');
                try {
                    docInfo = respValue.Split(';');
                } catch (Exception ex) {
                    docInfo = new string[] { "Failed to get document info",ex.Message};
                }
            }

            return docInfo;
        }

        /// <summary>method <c>SendRequest</c> request to url and get response</summary>
        /// <param name="=url">url destination</param>
        /// <param name="ShowMsgBox">Menampilkan message box atau tidak</param>
        /// <param name="AllowRetry">Menampilkan opsi retry atau tidak</param>
        /// <param name="RetryCount">Bantuan untuk menjumlahkan berapa kali retry</param>
        public string SendRequest(string url, bool ShowMsgBox = true, bool AllowRetry = true, int RetryCount = 0) {
            string resp = null;
            try {
                using (WebClient client = new WebClient()) {
                    //return client.DownloadString(new Uri(url));
                    resp = client.DownloadString(new Uri(url));
                }
            } catch (WebException ex) {
                if (ShowMsgBox) {
                    string message = "Error while receiving data from the server:\n" + ex.Message;
                    string box_title = "Silent Print - Warning Information";

                    var box_button = MessageBoxButtons.OK;
                    if (AllowRetry) {
                        box_button = MessageBoxButtons.RetryCancel;
                    }

                    var result = MessageBox.Show(null,
                        message, 
                        box_title, 
                        box_button, 
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Retry) {
                        if (RetryCount > 5) {
                            MessageBox.Show("Too many retries! Please try again later.", box_title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        } else {
                            resp = SendRequest(url, ShowMsgBox, AllowRetry, RetryCount+1);
                        }
                    } else {
                        resp = null;
                    }
                }
            }
            return resp;
        }
    }
}
