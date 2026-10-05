using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NNPTPZ1.Mathematics
{
    public class ComplexNumber
    {
        public double Real { get; set; }
        public double Imaginary { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ComplexNumber)
            {
                ComplexNumber x = obj as ComplexNumber;
                return x.Real == Real && x.Imaginary == Imaginary;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (Real, Imaginary).GetHashCode();
        }

        public readonly static ComplexNumber Zero = new ComplexNumber()
        {
            Real = 0,
            Imaginary = 0
        };

        public ComplexNumber Multiply(ComplexNumber otherNumber)
        {
            ComplexNumber a = this;
            // aRe*bRe + aRe*bIm*i + aIm*bRe*i + aIm*bIm*i*i
            return new ComplexNumber()
            {
                Real = a.Real * otherNumber.Real - a.Imaginary * otherNumber.Imaginary,
                Imaginary = (float)(a.Real * otherNumber.Imaginary + a.Imaginary * otherNumber.Real)
            };
        }
        public double GetAbsoluteValue()
        {
            return Math.Sqrt(Real * Real + Imaginary * Imaginary);
        }

        public ComplexNumber Add(ComplexNumber otherNumber)
        {
            ComplexNumber a = this;
            return new ComplexNumber()
            {
                Real = a.Real + otherNumber.Real,
                Imaginary = a.Imaginary + otherNumber.Imaginary
            };
        }
        public double GetAngleInDegrees()
        {
            return Math.Atan(Imaginary / Real);
        }
        public ComplexNumber Subtract(ComplexNumber otherNumber)
        {
            ComplexNumber a = this;
            return new ComplexNumber()
            {
                Real = a.Real - otherNumber.Real,
                Imaginary = a.Imaginary - otherNumber.Imaginary
            };
        }

        public override string ToString()
        {
            return $"({Real} + {Imaginary}i)";
        }

        internal ComplexNumber Divide(ComplexNumber otherNumber)
        {
            // (aRe + aIm*i) / (bRe + bIm*i)
            // ((aRe + aIm*i) * (bRe - bIm*i)) / ((bRe + bIm*i) * (bRe - bIm*i))
            //  bRe*bRe - bIm*bIm*i*i
            var numerator = this.Multiply(new ComplexNumber() { Real = otherNumber.Real, Imaginary = -otherNumber.Imaginary });
            var denominator = otherNumber.Real * otherNumber.Real + otherNumber.Imaginary * otherNumber.Imaginary;

            return new ComplexNumber()
            {
                Real = numerator.Real / denominator,
                Imaginary = (float)(numerator.Imaginary / denominator)
            };
        }
    }
}
