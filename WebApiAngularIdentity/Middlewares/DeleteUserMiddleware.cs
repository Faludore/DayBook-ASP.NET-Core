using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLibary.Models;
using DataAccessLibary.DataAccess;

namespace WebApiAngularIdentity.Middlewares
{
    public class DeleteUserMiddleware
    {
        private readonly RequestDelegate _next;
        public DeleteUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task InvokeAsync(HttpContext context, AuthenticationContext authenticationContext)
        {
            List<ApplicationUser> user = authenticationContext.ApplicationUsers.ToList();
            foreach (var item in user)
            {
                if (item.DnTDelete != null && CheckDnT(item.DnTDelete, DateTime.Now.ToString("dd/MM/yyyy")))
                {
                    using (authenticationContext)
                    {
                        authenticationContext.ApplicationUsers.Remove(item);
                        authenticationContext.SaveChanges();
                    }                 
                }
            }

           await _next(context);
        }

        private bool CheckDnT(string dnt1, string dnt2)
        {
            int[] arr_dnt1 = new int[3];
            int[] arr_dnt2 = new int[3];


            string[] elements1 = dnt1.Split(new char[] { '.' });
            string[] elements2 = dnt2.Split(new char[] { '.' });

            for (int i = 0; i < 3; i++)
            {
                arr_dnt1[i] = Convert.ToInt32(elements1[i]);
                arr_dnt2[i] = Convert.ToInt32(elements2[i]);
            }

            DateTime firstDate = new DateTime(arr_dnt1[2], arr_dnt1[1], arr_dnt1[0]);
            DateTime secondDate = new DateTime(arr_dnt2[2], arr_dnt2[1], arr_dnt2[0]);
            TimeSpan diff1 = secondDate - firstDate;

            int daydiff = Convert.ToInt32((secondDate - firstDate).TotalDays.ToString());

            if (daydiff > 2)
            {
                return true;
            }
            else
                return false;
        }
    }
}
