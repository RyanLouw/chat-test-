using CallQuality.Core.DataAccess.Context.Entities;
using CallQuality.Core.Manager.AssessmentsManager.Models;
using CallQuality.Core.Manager.QuestionsManager.Models;
using CallQuality.Core.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace CallQuality.Core.Helpers;

public class EmailHelper
{
    private readonly EmailSettings _emailSettings;
    private readonly IConfiguration _configuration;

    public EmailHelper(
        IOptions<EmailSettings> options,
        IConfiguration configuration)
    {
        _emailSettings = options.Value;
        _configuration = configuration;
    }

    public void SendEmail(EmailDisplayAssessment emailDisplayAssessment)
    {
        try
        {
            var fromAddress = new MailAddress(
                _emailSettings.FromEmail,
                _emailSettings.FromEmailName);

            using var smtp = new SmtpClient
            {
                Host = _emailSettings.Host,
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    fromAddress.Address,
                    _emailSettings.FromEmailPassword)
            };

            var demoEmails = GetDemoEmails();
            var isDevelopment = IsDevelopment();

            // Send email to the Agent
            if (!string.IsNullOrWhiteSpace(emailDisplayAssessment.Agent.Mail))
            {
                using var message = new MailMessage
                {
                    From = fromAddress
                };

                if (isDevelopment)
                {
                    foreach (var email in demoEmails)
                    {
                        message.To.Add(new MailAddress(email));
                    }
                }
                else
                {
                    message.To.Add(
                        new MailAddress(
                            emailDisplayAssessment.Agent.Mail.Trim()));
                }

                message.Subject =
                    "Call Quality Feedback - Health Window";

                message.Body =
                    GetAgentBody(
                        emailDisplayAssessment,
                        emailDisplayAssessment.Percentage);

                message.IsBodyHtml = true;

                smtp.Send(message);
            }
            else
            {
                Log.Error(
                    "Agent Email is empty: {ID_Guid}",
                    emailDisplayAssessment.Agent.ID_Guid);
            }

            // Send email to the Team Leader
            if (!string.IsNullOrWhiteSpace(
                    emailDisplayAssessment.TeamLeader.Mail))
            {
                using var message = new MailMessage
                {
                    From = fromAddress
                };

                if (isDevelopment)
                {
                    foreach (var email in demoEmails)
                    {
                        message.To.Add(new MailAddress(email));
                    }
                }
                else
                {
                    message.To.Add(
                        new MailAddress(
                            emailDisplayAssessment.TeamLeader.Mail.Trim()));
                }

                message.Subject =
                    "Call Quality Team Leader Report - Health Window";

                message.Body =
                    GetTeamLeaderBody(emailDisplayAssessment);

                message.IsBodyHtml = true;

                smtp.Send(message);
            }
            else
            {
                Log.Error(
                    "TeamLeader Email is empty: {ID_Guid}",
                    emailDisplayAssessment.TeamLeader.ID_Guid);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "SendEmail failed.");
        }
    }

    private static string GetAgentBody(
        EmailDisplayAssessment emailDisplayAssessment,
        string percentage)
    {
        try
        {
            string template = Templates.Email_Feedback_Body;
            string rowTemplate = Templates.EmailAddAssessmentRow;
            string rowTemplatesToReplace = "";

            foreach (EmailDisplayAssessmentRow row
                     in emailDisplayAssessment.rows)
            {
                string temp = rowTemplate;

                temp = temp.Replace(
                    "{_AssessedOn_}",
                    row.AssessedOn.ToShortDateString());

                temp = temp.Replace(
                    "{_Score_}",
                    row.score);

                temp = temp.Replace(
                    "{_Percentage_}",
                    percentage);

                rowTemplatesToReplace += temp;
            }

            template = template.Replace(
                "{_AssessmentsRow_}",
                rowTemplatesToReplace);

            template = template.Replace(
                "{_Feedback_}",
                emailDisplayAssessment.feedback);

            return template;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "GetAgentBody");
            return "";
        }
    }

    private string GetTeamLeaderBody(
        EmailDisplayAssessment emailDisplayAssessment)
    {
        try
        {
            string template =
                Templates.TeamLeaderFeedbackEmail;

            string rowTemplate =
                Templates.EmailAddAssessmentRow;

            string rowTemplatesToReplace = "";

            string id =
                emailDisplayAssessment.AssesmentId;

            string url;

            foreach (EmailDisplayAssessmentRow row
                     in emailDisplayAssessment.rows)
            {
                string temp = rowTemplate;

                temp = temp.Replace(
                    "{_AgentName_}",
                    emailDisplayAssessment.Agent.GivenName);

                temp = temp.Replace(
                    "{_AssessedOn_}",
                    row.AssessedOn.ToShortDateString());

                temp = temp.Replace(
                    "{_Score_}",
                    row.score);

                temp = temp.Replace(
                    "{_Percentage_}",
                    row.percentage);

                rowTemplatesToReplace += temp;
            }

            template = template.Replace(
                "{__Name__}",
                emailDisplayAssessment.TeamLeader.DisplayName);

            template = template.Replace(
                "{__AgentName__}",
                emailDisplayAssessment.Agent.DisplayName);

            template = template.Replace(
                "{_AssessmentsRow_}",
                rowTemplatesToReplace);

            template = template.Replace(
                "{_TotalAssessments_}",
                emailDisplayAssessment.rows.Count.ToString());

            template = template.Replace(
                "{_Feedback_}",
                emailDisplayAssessment.feedback);

            if (string.IsNullOrWhiteSpace(id))
            {
                url = "no link could be found";
            }
            else
            {
                url =
                    $"{_emailSettings.ViewOperatorAssessmentURL}{id}";
            }

            template = template.Replace(
                "{__Url__}",
                url);

            return template;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "GetTeamLeaderBody");
            return "";
        }
    }

    public static string BuildCombinedFeedbackHtml(
        string? autoFeedback,
        string? additionalFeedback)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(autoFeedback))
        {
            sb.AppendLine(
                "<p><strong>System Feedback</strong></p>");

            sb.AppendLine(autoFeedback.Trim());
        }

        if (!string.IsNullOrWhiteSpace(additionalFeedback))
        {
            var additionalList =
                BuildHtmlList(additionalFeedback);

            if (!string.IsNullOrEmpty(additionalList))
            {
                sb.AppendLine(
                    "<p><strong>Additional Feedback</strong></p>");

                sb.AppendLine(additionalList);
            }
        }

        return sb.ToString().Trim();
    }

    private static string BuildHtmlList(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var lines = text
            .Replace(
                "<br>",
                "\n",
                StringComparison.OrdinalIgnoreCase)
            .Replace(
                "<br/>",
                "\n",
                StringComparison.OrdinalIgnoreCase)
            .Replace(
                "<br />",
                "\n",
                StringComparison.OrdinalIgnoreCase)
            .Split(
                new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(WebUtility.HtmlEncode)
            .ToList();

        if (!lines.Any())
            return string.Empty;

        var sb = new StringBuilder();

        sb.AppendLine("<ul>");

        foreach (var line in lines)
        {
            sb.AppendLine($"  <li>{line}</li>");
        }

        sb.AppendLine("</ul>");

        return sb.ToString();
    }

    public void SendReassessmentFeedbackEmail(
        ReassessmentDTO reassessment,
        ADUser user)
    {
        try
        {
            if (reassessment is null)
            {
                Log.Error(
                    "SendReassessmentFeedbackEmail failed: reassessment is null.");

                return;
            }

            if (user is null)
            {
                Log.Error(
                    "SendReassessmentFeedbackEmail failed: assessor user is null. AssessmentID: {AssessmentID}",
                    reassessment.AssessmentID);

                return;
            }

            var feedbackQuestions =
                reassessment.Questions?
                    .Where(question =>
                        !string.IsNullOrWhiteSpace(
                            question.ReassessorNote))
                    .ToList()
                ?? new List<QuestionAnswerDTO>();

            if (feedbackQuestions.Count == 0)
            {
                Log.Information(
                    "Reassessment feedback email not sent because no reassessor notes were provided. AssessmentID: {AssessmentID}",
                    reassessment.AssessmentID);

                return;
            }

            var assessorEmail = user.Mail?.Trim();

            if (string.IsNullOrWhiteSpace(assessorEmail))
            {
                Log.Error(
                    "Assessor email is empty. AssessmentID: {AssessmentID}",
                    reassessment.AssessmentID);

                return;
            }

            var isDevelopment = IsDevelopment();

            var demoEmails = GetDemoEmails();

            if (isDevelopment &&
                demoEmails.Length == 0)
            {
                Log.Error(
                    "No demo email addresses are configured. AssessmentID: {AssessmentID}",
                    reassessment.AssessmentID);

                return;
            }

            if (!MailAddress.TryCreate(
                    _emailSettings.FromEmail?.Trim(),
                    out var fromAddress))
            {
                Log.Error(
                    "Invalid sender email address: {SenderEmail}",
                    _emailSettings.FromEmail);

                return;
            }

            using var smtp = new SmtpClient
            {
                Host = _emailSettings.Host,
                Port = 587,
                EnableSsl = true,
                DeliveryMethod =
                    SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials =
                    new NetworkCredential(
                        fromAddress.Address,
                        _emailSettings.FromEmailPassword)
            };

            using var message = new MailMessage
            {
                From = fromAddress,
                IsBodyHtml = true
            };

            var emailBody =
                GetReassessmentFeedbackBody(
                    reassessment,
                    feedbackQuestions);

            if (string.IsNullOrWhiteSpace(emailBody))
            {
                Log.Error(
                    "Reassessment email body could not be generated. AssessmentID: {AssessmentID}",
                    reassessment.AssessmentID);

                return;
            }

            if (isDevelopment)
            {
                foreach (var demoEmail in demoEmails)
                {
                    if (!MailAddress.TryCreate(
                            demoEmail,
                            out var demoAddress))
                    {
                        Log.Error(
                            "Invalid demo email address: {DemoEmail}",
                            demoEmail);

                        continue;
                    }

                    message.To.Add(demoAddress);
                }

                if (message.To.Count == 0)
                {
                    Log.Error(
                        "No valid demo recipients found. AssessmentID: {AssessmentID}",
                        reassessment.AssessmentID);

                    return;
                }

                message.Subject =
                    $"[DEMO - Intended for: {assessorEmail}] " +
                    "Call Quality Reassessment Feedback - Health Window";

                message.Body = $@"
                    <div style=""padding:10px;margin-bottom:15px;border:1px solid #d6b656;background-color:#fff2cc;"">
                        <strong>Demo email</strong><br />
                        This email would be sent to:
                        <strong>{Html(assessorEmail)}</strong>
                    </div>

                    {emailBody}";
            }
            else
            {
                if (!MailAddress.TryCreate(
                        assessorEmail,
                        out var assessorAddress))
                {
                    Log.Error(
                        "Invalid assessor email address: {AssessorEmail}. AssessmentID: {AssessmentID}",
                        assessorEmail,
                        reassessment.AssessmentID);

                    return;
                }

                message.To.Add(assessorAddress);

                message.Subject =
                    "Call Quality Reassessment Feedback - Health Window";

                message.Body = emailBody;
            }

            smtp.Send(message);

            Log.Information(
                "Reassessment feedback email sent. AssessmentID: {AssessmentID}, IntendedRecipient: {Recipient}, IsDevelopment: {IsDevelopment}",
                reassessment.AssessmentID,
                assessorEmail,
                isDevelopment);
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                "SendReassessmentFeedbackEmail failed. AssessmentID: {AssessmentID}",
                reassessment?.AssessmentID);
        }
    }

    private static string GetReassessmentFeedbackBody(
        ReassessmentDTO reassessment,
        List<QuestionAnswerDTO> failedQuestions)
    {
        try
        {
            if (reassessment is null)
            {
                Log.Error(
                    "GetReassessmentFeedbackBody failed: reassessment is null.");

                return string.Empty;
            }

            if (failedQuestions == null ||
                failedQuestions.Count == 0)
            {
                Log.Warning(
                    "GetReassessmentFeedbackBody received no failed questions. AssessmentID: {AssessmentID}",
                    reassessment.AssessmentID);

                return string.Empty;
            }

            var questionRows =
                BuildReassessmentFeedbackQuestionRows(
                    failedQuestions);

            var questionCountText =
                failedQuestions.Count == 1
                    ? "1 question requires attention."
                    : $"{failedQuestions.Count} questions require attention.";

            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            </head>

            <body style=""margin:0;padding:0;background-color:#f4f6f8;font-family:Arial,Helvetica,sans-serif;color:#333333;"">

                <div style=""max-width:850px;margin:0 auto;padding:24px;"">

                    <div style=""background-color:#ffffff;border:1px solid #e5e7eb;border-radius:10px;overflow:hidden;"">

                        <div style=""background-color:#0f766e;color:#ffffff;padding:24px;"">

                            <h1 style=""margin:0;font-size:24px;"">
                                New Reassessment Feedback
                            </h1>

                            <p style=""margin:8px 0 0 0;font-size:14px;"">
                                Feedback was provided for questions that did not pass reassessment.
                            </p>

                        </div>

                        <div style=""padding:24px;"">

                            <p style=""font-size:15px;line-height:1.5;margin-top:0;"">
                                Please review the reassessment feedback below.
                                <strong>{Html(questionCountText)}</strong>
                            </p>

                            <table
                                role=""presentation""
                                style=""width:100%;border-collapse:collapse;font-size:14px;"">

                                <thead>
                                    <tr>
                                        <th style=""width:60%;background-color:#f8fafc;text-align:left;padding:12px;border:1px solid #e2e8f0;color:#334155;"">
                                            Question
                                        </th>

                                        <th style=""width:40%;background-color:#f8fafc;text-align:left;padding:12px;border:1px solid #e2e8f0;color:#334155;"">
                                            Reassessor Notes
                                        </th>
                                    </tr>
                                </thead>

                                <tbody>
                                    {questionRows}
                                </tbody>

                            </table>

                        </div>

                        <div style=""padding:18px 24px;background-color:#f8fafc;font-size:12px;color:#64748b;"">
                            This is an automated Call Quality reassessment feedback notification.
                        </div>

                    </div>

                </div>

            </body>
            </html>";
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                "GetReassessmentFeedbackBody failed. AssessmentID: {AssessmentID}",
                reassessment?.AssessmentID);

            return string.Empty;
        }
    }

    private static string BuildReassessmentFeedbackQuestionRows(
        List<QuestionAnswerDTO> failedQuestions)
    {
        if (failedQuestions == null ||
            failedQuestions.Count == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();

        foreach (var question in failedQuestions)
        {
            if (question is null)
                continue;

            var questionText =
                question.QuestionValue?.Trim();

            if (string.IsNullOrWhiteSpace(questionText))
            {
                questionText =
                    "Question details unavailable";
            }

            var note =
                question.ReassessorNote?.Trim();

            if (string.IsNullOrWhiteSpace(note))
            {
                note =
                    "No reassessor note was provided.";
            }

            sb.AppendLine($@"
            <tr>
                <td style=""padding:12px;border:1px solid #e2e8f0;vertical-align:top;font-weight:bold;color:#111827;"">
                    {Html(questionText)}
                </td>

                <td style=""padding:12px;border:1px solid #e2e8f0;vertical-align:top;white-space:pre-line;color:#374151;"">
                    {Html(note)}
                </td>
            </tr>");
        }

        return sb.ToString();
    }

    private string[] GetDemoEmails()
    {
        return _emailSettings.DemoEmails
            ?.Split(
                new[] { ',', ';', '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries)
            .Select(email => email.Trim())
            .Where(email =>
                !string.IsNullOrWhiteSpace(email))
            .ToArray()
            ?? Array.Empty<string>();
    }

    private bool IsDevelopment()
    {
        return bool.TryParse(
                   _configuration["IsDevelopment"],
                   out var isDevelopment)
               && isDevelopment;
    }

    private static string Html(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "";
        }

        return WebUtility.HtmlEncode(
            value.Trim());
    }
}