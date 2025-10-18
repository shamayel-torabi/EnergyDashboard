using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Data
{
    public class FarsiIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError()
        {
            return new IdentityError
            {
                Code = nameof(DefaultError),
                Description = $"یک خطای ناشناخته رخ داده است."
            };
        }
        public override IdentityError ConcurrencyFailure()
        {
            return new IdentityError
            {
                Code = nameof(ConcurrencyFailure),
                Description = "Optimistic concurrency failure, object has been modified."
            };
        }
        public override IdentityError PasswordMismatch() { 
            return new IdentityError { 
                Code = nameof(PasswordMismatch), 
                Description = "گذرواژه نادرست."
            };
        }
        public override IdentityError InvalidToken() { 
            return new IdentityError { 
                Code = nameof(InvalidToken), 
                Description = "توکن نادرست."
            }; 
        }
        public override IdentityError LoginAlreadyAssociated() {
            return new IdentityError {
                Code = nameof(LoginAlreadyAssociated),
                Description = "کاربر با این مشخصات وجود دارد."
            };
        }
        public override IdentityError InvalidUserName(string userName) { 
            return new IdentityError {
                Code = nameof(InvalidUserName),
                Description = $"نام کاربری '{userName}' معتر نیست, باید از حروف و عدد تشکیل شود." 
            };
        }
        public override IdentityError InvalidEmail(string email) {
            return new IdentityError { 
                Code = nameof(InvalidEmail),
                Description = $"رایانامه '{email}' معتبر نیست." 
            };
        }
        public override IdentityError DuplicateUserName(string userName) {
            return new IdentityError {
                Code = nameof(DuplicateUserName),
                Description = $"نام کاربری  '{userName}' از قبل وجود دارد." 
            };
        }
        public override IdentityError DuplicateEmail(string email) {
            return new IdentityError {
                Code = nameof(DuplicateEmail),
                Description = $"رایانامه '{email}' از قبل وجود دارد." 
            };
        }
        public override IdentityError InvalidRoleName(string role) {
            return new IdentityError {
                Code = nameof(InvalidRoleName),
                Description = $"نقش '{role}' نا معتبر است." 
            };
        }
        public override IdentityError DuplicateRoleName(string role) {
            return new IdentityError {
                Code = nameof(DuplicateRoleName),
                Description = $"نقش '{role}' از قبل وجود دارد." 
            };
        }
        public override IdentityError UserAlreadyHasPassword() {
            return new IdentityError {
                Code = nameof(UserAlreadyHasPassword),
                Description = "کاربر هم اکنون گذرواژه دارد." 
            };
        }
        public override IdentityError UserLockoutNotEnabled() {
            return new IdentityError {
                Code = nameof(UserLockoutNotEnabled),
                Description = "قفل حساب برای این کاربر فعال نیست." 
            };
        }
        public override IdentityError UserAlreadyInRole(string role) {
            return new IdentityError {
                Code = nameof(UserAlreadyInRole),
                Description = $"کاربر در نقش '{role}' قرار دارد."
            };
        }
        public override IdentityError UserNotInRole(string role) {
            return new IdentityError {
                Code = nameof(UserNotInRole),
                Description = $"کاربر در نقش '{role}' نیست."
            };
        }
        public override IdentityError PasswordTooShort(int length) {
            return new IdentityError { 
                Code = nameof(PasswordTooShort), 
                Description = $"گذر واژه باید حداقل {length} حرف داشته باشد."
            };
        }
        public override IdentityError PasswordRequiresNonAlphanumeric() {
            return new IdentityError {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = "گذرواژه باید حتما یک حرف یا عدد داشته باشد."
            };
        }
        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) {
            return new IdentityError {
                Code = nameof(PasswordRequiresUniqueChars),
                Description = "گذرواژه باید حتما حروف یکتا داشته باشد."
            };
        }
        public override IdentityError PasswordRequiresDigit() {
            return new IdentityError {
                Code = nameof(PasswordRequiresDigit),
                Description = "گذر واژه باید حتما یک عدد ('0'-'9') داشته باشد."
            };
        }
        public override IdentityError PasswordRequiresLower() {
            return new IdentityError {
                Code = nameof(PasswordRequiresLower),
                Description = "گذر وازه باید حتما یک حرف کوچک ('a'-'z') داشته باشد."
            };
        }
        public override IdentityError PasswordRequiresUpper() {
            return new IdentityError {
                Code = nameof(PasswordRequiresUpper),
                Description = "گذر واژه باید حتما یک حرف بزرگ ('A'-'Z') داشته باشد."
            };
        }
        public override IdentityError RecoveryCodeRedemptionFailed() {
            return new IdentityError {
                Code = nameof(RecoveryCodeRedemptionFailed),
                Description = "خطا در کد بازیابی."
            };
        }
    }

}
