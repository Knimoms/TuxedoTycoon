using System;

public struct Tuxdollar //actual = Value*1000 to the power of magnitude.
{
    public float Value; // outside of this class this Value will only be between 0 and 1000.
    public string Magnitude // a == 1, b == 2 ... z == 26, aa = 27
    {
        get
        {
            return _magnitude;
        }
        set
        {
            foreach (char c in value)
            {
                if (!MagnitudeLetters.Contains(c))
                    return;
            }

            _magnitude = value;
        }
    }

    private string _magnitude;

    private static char[] _magnitude_letters = new char[26];
    private static string MagnitudeLetters = "";

    public Tuxdollar(float Value, string Magnitude)
    {
        if (_magnitude_letters[0] == '\0')
        {
            for (int i = 0; i < _magnitude_letters.Length; i++)
            {
                _magnitude_letters[i] = (char)('a' + i);
            }
            MagnitudeLetters = new string(_magnitude_letters);
        }

        this.Value = Value;

        if(Magnitude == null)
        {
            this._magnitude = "";
            return;
        }

        foreach (char c in Magnitude)
        {
            if (!MagnitudeLetters.Contains(c))
            {
                Magnitude = "";
                break;
            }
        }

        this._magnitude = Magnitude;
    }

    public Tuxdollar(float Value)
    {

        if (_magnitude_letters[0] == '\0')
        {
            for (int i = 0; i < _magnitude_letters.Length; i++)
            {
                _magnitude_letters[i] = (char)('a' + i);
            }
            MagnitudeLetters = new string(_magnitude_letters);
        }
        this.Value = Value;
        this._magnitude = "";
    }

    public static Tuxdollar operator +(Tuxdollar left, Tuxdollar right)
    {
        int lM = left.MagnitudeToInteger();
        int rM = right.MagnitudeToInteger();

        if (lM < rM)
            left._convert_to_larger_magnitude(right.Magnitude);
        if (rM < lM)
            right._convert_to_larger_magnitude(left.Magnitude);

        Tuxdollar result = new Tuxdollar(left.Value + right.Value, left.Magnitude);
        while (result._check_for_magnitude_change()) ;

        return result;
    }

    public static Tuxdollar operator -(Tuxdollar left, Tuxdollar right)
    {
        int lM = left.MagnitudeToInteger();
        int rM = right.MagnitudeToInteger();

        if (lM < rM)
            left._convert_to_larger_magnitude(right.Magnitude);
        if (rM < lM)
            right._convert_to_larger_magnitude(left.Magnitude);

        Tuxdollar result = new Tuxdollar(left.Value - right.Value, left.Magnitude);
        while (result._check_for_magnitude_change()) ;

        return result;
    }

    public static Tuxdollar operator -(Tuxdollar left)
    {
        return left*-1;
    }

    public static Tuxdollar operator *(Tuxdollar left, Tuxdollar right)
    {
        Tuxdollar result = new Tuxdollar(left.Value * right.Value, IntegerToMagnitudeString(left.MagnitudeToInteger()+right.MagnitudeToInteger()));
        while (result._check_for_magnitude_change()) ;

        return result;
    }

    public static Tuxdollar operator *(Tuxdollar left, float right)
    {
        Tuxdollar result = new Tuxdollar(left.Value * right, left.Magnitude);
        while (result._check_for_magnitude_change()) ;

        return result;
    }

    public static Tuxdollar operator *(float left, Tuxdollar right)
    {
        Tuxdollar result = new Tuxdollar(right.Value * left, right.Magnitude);
        while (result._check_for_magnitude_change()) ;

        return result;
    }

    public static Tuxdollar operator /(Tuxdollar left, float right)
    {
        Tuxdollar result = new Tuxdollar(left.Value / right, left.Magnitude);
        while (result._check_for_magnitude_change()) ;

        return result;
    }

    public static Tuxdollar operator /(float left, Tuxdollar right)
    {
        Tuxdollar result = new Tuxdollar(right.Value / left, right.Magnitude);
        while (result._check_for_magnitude_change()) ;

        return result;
    }

    public static Tuxdollar operator /(Tuxdollar left, Tuxdollar right)
    {
        Tuxdollar result = new Tuxdollar(right.Value / left.Value, left.Magnitude);
        while (result._check_for_magnitude_change()) ;

        return result;
    }

    public static bool operator <(Tuxdollar left, Tuxdollar right)
    {
        return (left.MagnitudeToInteger() < right.MagnitudeToInteger() || left.Magnitude == right.Magnitude && left.Value < right.Value);
    }

    public static bool operator >(Tuxdollar left, Tuxdollar right)
    {
        return (left.MagnitudeToInteger() > right.MagnitudeToInteger() || left.Magnitude == right.Magnitude && left.Value > right.Value);
    }
    public static bool operator >=(Tuxdollar left, Tuxdollar right)
    {
        return (left > right || left == right);
    }
    public static bool operator <=(Tuxdollar left, Tuxdollar right)
    {
        return (left < right || left == right);
    }

    public static bool operator ==(Tuxdollar left, Tuxdollar right) //Allows tolerance of up to 0.02f
    {
        return (Math.Abs(left.Value - right.Value) < 0.02f && left.Magnitude == right.Magnitude);
    }

    public static bool operator !=(Tuxdollar left, Tuxdollar right) //Allows tolerance of up to 0.02f
    {
        return (Math.Abs(left.Value - right.Value) > 0.02f || left.Magnitude != right.Magnitude);
    }

    public override bool Equals(object o)
    {   
        if(!(o is Tuxdollar)) return false;
        Tuxdollar obj = (Tuxdollar)o;
        return (this.Value == obj.Value && this.Magnitude == obj.Magnitude);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    private void _convert_to_larger_magnitude(string magnitude) //converts the Tuxedo to a higher magnitude for operator calculations between two Tuxedo's
    {
        int mI = MagnitudeToInteger(magnitude);
        if (this.MagnitudeToInteger() > MagnitudeToInteger(magnitude))
            return;

        int magnitudeDifference = MagnitudeToInteger(magnitude) - this.MagnitudeToInteger();

        this = new Tuxdollar(this.Value / (float)Math.Pow(1000, magnitudeDifference), magnitude);
    }

    private bool _check_for_magnitude_change()  //checks if the Tuxedo has grown or shrank to a Value where a magnitude change is viable. If yes, it also carries out the task.
    {
        float absValue = Math.Abs(this.Value);

        if (absValue < 1000.0f && absValue >= 1.0f || this.Magnitude == "" && absValue < 1.0f)
            return false;

        if (absValue > 1000)
        {
            this.Value *= 0.001f;
            this._integer_to_magnitude(this.MagnitudeToInteger() + 1);
        }
        else
        {
            this.Value *= 1000f;
            this._integer_to_magnitude(this.MagnitudeToInteger() - 1);
        }

        return true;
    }

    public int MagnitudeToInteger() //converts the Tuxedo's magnitude to an integer
    {
        int x = 0;
        for (int i = 0; i < this.Magnitude.Length; i++)
        {
            x += (this.Magnitude[Magnitude.Length - 1 - i] - 96) * (int)Math.Pow(26, i);
        }

        return x;
    }

    public static int MagnitudeToInteger(string magnitude) // converts the in the parameter given magnitude to an integer
    {
        int x = 0;

        foreach (char c in magnitude)
        {
            if (!MagnitudeLetters.Contains(c)) return -1;
        }
        for (int i = 0; i < magnitude.Length; i++)
        {
            x += (magnitude[magnitude.Length - 1 - i] - 96) * (int)Math.Pow(26, i);
        }

        return x;
    }

    private void _integer_to_magnitude(int i) // changes the Tuxdollar's magnitude to the value of the in the parameter given integer
    {
        string magnitude = "";

        if (i <= 0)
        {
            this.Magnitude = magnitude;
            return;
        }
        int remainder;

        while (i != 0)
        {
            remainder = i % 26;
            if (remainder == 0)
            {
                remainder = 26;
                i--;
            }

            magnitude = MagnitudeLetters[remainder - 1] + magnitude;

            i /= 26;

        }

        this.Magnitude = magnitude;
    }

    public static string IntegerToMagnitudeString(int i) // returns the in the parameter given integer as a magnitude-string
    {
        string magnitude = "";

        if (i <= 0)
        {
            return magnitude;
        }
        int remainder;

        while (i != 0)
        {
            remainder = i % 26;
            if (remainder == 0)
            {
                remainder = 26;
                i--;
            }

            magnitude = MagnitudeLetters[remainder - 1] + magnitude;

            i /= 26;

        }

        return magnitude;
    }

    public double ActualValue() // returns the actual value of the Tuxdollar as a double
    {
        return this.Value * Math.Pow(1000, this.MagnitudeToInteger());
    }

    public override string ToString()
    {
        return $"{this.Value.ToString("F2")}{this.Magnitude}$";
    }
}
