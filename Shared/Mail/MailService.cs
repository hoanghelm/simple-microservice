﻿using Common.Extensions;
using MailKit.Net.Smtp;
using MailKit.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Mail
{
	public class MailService : IMailService
	{
		private readonly MailServerSetting _mailServerSetting;

		public MailService(MailServerSetting mailServerSetting)
		{
			_mailServerSetting = mailServerSetting;
		}

		public async Task<bool> SendAsync(MailMessage mailMessage, bool keepReceiver = false)
		{
			try
			{
				mailMessage.From = _mailServerSetting.FromAddress;
				mailMessage.DisplayName = _mailServerSetting.DisplayName;
				mailMessage.HtmlMessage = ReplaceContentTest(mailMessage);
				mailMessage.To = ReplaceEmailTest(mailMessage.To);
				mailMessage.Cc = ReplaceEmailTest(mailMessage.Cc);
				mailMessage.Bcc = ReplaceEmailTest(mailMessage.Bcc);
				mailMessage.Subject = ReplaceSubjectTest(mailMessage.Subject);
				mailMessage.HtmlMessage = mailMessage.HtmlMessage;

				if (mailMessage.To.IsEmpty() && !keepReceiver)
				{
					if (!mailMessage.Cc.IsEmpty())
					{
						mailMessage.To = mailMessage.Cc;
						mailMessage.Cc = string.Empty;
					}
					else if (!mailMessage.Bcc.IsEmpty())
					{
						mailMessage.To = mailMessage.Bcc;
						mailMessage.Bcc = string.Empty;
					}
					else
					{
						return false;
					}
				}

				// Send mail
				using (var client = new SmtpClient())
				{
					client.ServerCertificateValidationCallback = (s, c, h, e) => true;
					await client.ConnectAsync(_mailServerSetting.Host, _mailServerSetting.Port, SecureSocketOptions.Auto);

					client.AuthenticationMechanisms.Remove("XOAUTH2");
					await client.AuthenticateAsync(_mailServerSetting.UserName, _mailServerSetting.Password);
					await client.SendAsync(mailMessage.ToMimeMessage());

					client.Disconnect(true);
				}

				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		private string ReplaceEmailTest(string emailAdress)
		{
			if (string.IsNullOrWhiteSpace(emailAdress)) return string.Empty;

			return string.IsNullOrWhiteSpace(_mailServerSetting.EmailTest)
				? emailAdress
				: _mailServerSetting.EmailTest;
		}

		private string ReplaceSubjectTest(string subject)
		{
			if (!_mailServerSetting.EmailTest.IsEmpty())
			{
				return "[RoShop Email Test] - " + subject;
			}

			return subject;
		}

		private string ReplaceContentTest(MailMessage mailMessage)
		{
			return string.IsNullOrWhiteSpace(_mailServerSetting.EmailTest) ?
				mailMessage.HtmlMessage :
				string.Concat(mailMessage.HtmlMessage,
				"<br/><br/>--"
				+ "<br/><br/> PLEASE NOTE: THIS EMAIL WAS SENT FOR TESTING PURPOSE. DO NOT DO ANYTHING."
				+ "<br/><br/>Testing purpose: Email addresses will be sent to: <br/><br/> "
				+ "To: " + mailMessage.To + "<br/><br/>"
				+ "Bcc: " + mailMessage.Bcc + "<br/><br/>"
				+ "Cc: " + mailMessage.Cc
				+ (mailMessage.ExtensionString.IsEmpty() ? "" : "<br/><br/>--<br/><br/>" + mailMessage.ExtensionString));
		}
	}
}
