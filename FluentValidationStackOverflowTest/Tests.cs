using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using NUnit.Framework;

namespace FluentValidationStackOverflowTest
{
    public class GridLayoutModuleDataModel
    {
        public List<GridLayoutRowModel> Rows { get; set; }
    }

    public class GridLayoutModel
    {
        public List<GridLayoutRowModel> Rows { get; set; }
    }

    public class GridLayoutRowModel
    {
        public List<GridLayoutModuleDataModel> Modules { get; set; }
    }

    public class GridLayoutModelValidator : AbstractValidator<GridLayoutModel>
    {
        public GridLayoutModelValidator()
        {
            RuleFor(layout => layout.Rows).NotEmpty().SetCollectionValidator(new GridLayoutRowModelValidator());
        }
    }

    public class GridLayoutRowModelValidator : AbstractValidator<GridLayoutRowModel>
    {
        public GridLayoutRowModelValidator()
        {
            RuleFor(row => row.Modules).Cascade(CascadeMode.Continue).NotEmpty()
                .SetCollectionValidator(new GridLayoutModuleDataModelValidator());
        }
    }

    public class GridLayoutModuleDataModelValidator : AbstractValidator<GridLayoutModuleDataModel>
    {
        public GridLayoutModuleDataModelValidator()
        {
            RuleFor(module => module.Rows)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .SetCollectionValidator(new GridLayoutRowModelValidator())
                .When(x => x.Rows.Count > 0);
        }
    }

    [TestFixture]
    public class ValidatorTests
    {
        [Test]
        public void BasicRecursiveTest()
        {
            var model = new GridLayoutModel();
            var validator = new GridLayoutModelValidator();
            validator.ValidateAndThrow(model); //this throws a stackoverflow exception
        }
    }
}
