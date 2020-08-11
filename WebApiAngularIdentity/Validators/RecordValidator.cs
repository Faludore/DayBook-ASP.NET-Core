using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using WebApiAngularIdentity.Models;

namespace WebApiAngularIdentity.Validators
{
    public class RecordValidator : AbstractValidator<Record>
    {
        public RecordValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(50);

            RuleFor(x => x.Text).NotEmpty().MaximumLength(500);

        }
    }
}
