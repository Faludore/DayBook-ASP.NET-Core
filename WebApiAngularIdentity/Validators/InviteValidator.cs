using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLibary.Models;
using DataAccessLibary.DataAccess;

namespace WebApiAngularIdentity.Validators
{
    public class InviteValidator : AbstractValidator<Invite>
    {
        public InviteValidator()
        {
            
            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(30);

        }
    }
}
