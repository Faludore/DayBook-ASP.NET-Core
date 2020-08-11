using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiAngularIdentity.Models;

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
