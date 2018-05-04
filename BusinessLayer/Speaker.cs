using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer
{
    /// <summary>
    /// Represents a single speaker
    /// </summary>
    public class Speaker
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public int? YearsExperience { get; set; }
		public bool HasBlog { get; set; }
		public string BlogURL { get; set; }
		public WebBrowser Browser { get; set; }
		public List<string> Certifications { get; set; }
		public string Employer { get; set; }
		public int RegistrationFee { get; set; }
		public List<BusinessLayer.Session> Sessions { get; set; }


        public void ValidateData()
        {
            if (string.IsNullOrEmpty(FirstName)) throw new ArgumentNullException("First Name is required.");
            if (string.IsNullOrEmpty(LastName)) throw new ArgumentNullException("Last Name is required.");
            if (string.IsNullOrEmpty(Email)) throw new ArgumentNullException("Email is required.");
            if (Sessions.Count() == 0) throw new ArgumentException("Can't register speaker with no sessions to present.");
        }

        public void ValidateRegistration()
        {
            ValidateData();

            bool speakerAppearsQualified = CheckSpecialRequirements() || !ObviousRedFlags();
            if (!speakerAppearsQualified)
            {
                throw new SpeakerDoesntMeetRequirementsException("This speaker doesn't meet our standards.");
            }

            ApproveSessions();
        }

        private bool SessionIsAboutOldTechnology(Session session)
        {
            string[] oldTechnologies = new string[] { "Cobol", "Punch Cards", "Commodore", "VBScript" };
            foreach (var oldTech in oldTechnologies)
            {
                if (session.Title.Contains(oldTech) || session.Description.Contains(oldTech)) return true;
            }
            return false;
        }


        private void ApproveSessions()
        {
            foreach (var session in Sessions)
            {
                session.Approved = !SessionIsAboutOldTechnology(session);
            }
            bool noSessionsApproved = Sessions.Where(s => s.Approved).Count() == 0;
            if (noSessionsApproved) throw new NoSessionsApprovedException("No sessions approved");
        }


        private bool ObviousRedFlags()
        {
            //need to get just the domain from the email
            string emailDomain = Email.Split('@').Last();
            var ancientEmailDomains = new List<string>() { "aol.com", "hotmail.com", "prodigy.com", "compuserve.com" };
            return (ancientEmailDomains.Contains(emailDomain) || ((Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9)));
        }

        private bool CheckSpecialRequirements()
        {
            if (YearsExperience > 10) return true;
            if (HasBlog) return true;
            if (Certifications.Count() > 3) return true;
            var preferredEmployers = new List<string>() { "Microsoft", "Google", "Fog Creek Software", "37Signals" };
            if (preferredEmployers.Contains(Employer)) return true;
            return false;
        }

        /// <summary>
        /// Register a speaker
        /// </summary>
        /// <returns>speakerID</returns>
        public int? Register(IRepository repository)
		{
			int? speakerId = null;
            ValidateRegistration();
            speakerId = repository.SaveSpeaker(this);
            return speakerId;            			
		}

		#region Custom Exceptions
		public class SpeakerDoesntMeetRequirementsException : Exception
		{
			public SpeakerDoesntMeetRequirementsException(string message)
				: base(message)
			{
			}

			public SpeakerDoesntMeetRequirementsException(string format, params object[] args)
				: base(string.Format(format, args)) { }
		}

		public class NoSessionsApprovedException : Exception
		{
			public NoSessionsApprovedException(string message)
				: base(message)
			{
			}
		}
		#endregion
	}
}