using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIronChef.Application.Recipes.Commands.DeleteRecipe
{
    public class DeleteRecipeCommand
    {
        public int Id { get; set; }

        public bool IsValid()
        {
            return ValidationHelper.IsIdValid(Id);
        }
    }
}
