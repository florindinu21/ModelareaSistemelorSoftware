using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace QA.Infrastructure.Domain
{
    [Table("AspNetUsers")]
    public sealed class ApplicationUser : IdentityUser<int>
    {
        public ApplicationUser()
        {
        }


        [Required]
        public DateTime RegistrationDate { get; set; }

        [InverseProperty("User")]
        public ICollection<Question> UserQuestions { get; set; }

        [InverseProperty("User")]
        public ICollection<Answer> UserAnswers { get; set; }

        [InverseProperty("User")]
        public ICollection<Vote> UserVotes { get; set; }
    }
}
