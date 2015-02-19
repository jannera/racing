using UnityEngine;
using System.Collections;

public class Matrix3x3
{
    public static Matrix3x3 identity = new Matrix3x3(1, 0, 0, 0, 1, 0, 0, 0, 1);
    public static Matrix3x3 zero = new Matrix3x3(0, 0, 0, 0, 0, 0, 0, 0, 0);

    public float[,] matrix;

    public Matrix3x3()
    {
        matrix = new float[3, 3];
        matrix[0, 0] = 0;
        matrix[1, 0] = 0;
        matrix[2, 0] = 0;
        matrix[0, 1] = 0;
        matrix[1, 1] = 0;
        matrix[2, 1] = 0;
        matrix[0, 2] = 0;
        matrix[1, 2] = 0;
        matrix[2, 2] = 0;
    }
    public Matrix3x3(float a11, float a12, float a13, float a21, float a22, float a23)
    {
        matrix = new float[3, 3];
        matrix[0, 0] = a11;
        matrix[1, 0] = a12;
        matrix[2, 0] = a13;
        matrix[0, 1] = a21;
        matrix[1, 1] = a22;
        matrix[2, 1] = a23;
        matrix[0, 2] = 0;
        matrix[1, 2] = 0;
        matrix[2, 2] = 1;
    }
    public Matrix3x3(Vector3 a1, Vector3 a2)
    {
        matrix = new float[3, 3];
        matrix[0, 0] = a1.x;
        matrix[1, 0] = a1.y;
        matrix[2, 0] = a1.z;
        matrix[0, 1] = a2.x;
        matrix[1, 1] = a2.y;
        matrix[2, 1] = a2.z;
        matrix[0, 2] = 0;
        matrix[1, 2] = 0;
        matrix[2, 2] = 1;
    }
    public Matrix3x3(float a11, float a12, float a13, float a21, float a22, float a23, float a31, float a32, float a33)
    {
        matrix = new float[3, 3];
        matrix[0, 0] = a11;
        matrix[1, 0] = a12;
        matrix[2, 0] = a13;
        matrix[0, 1] = a21;
        matrix[1, 1] = a22;
        matrix[2, 1] = a23;
        matrix[0, 2] = a31;
        matrix[1, 2] = a32;
        matrix[2, 2] = a33;
    }

    //MATRIX MULTIPLICATION
    public static Matrix3x3 operator *(Matrix3x3 m1, Matrix3x3 m2)
    {
        float a11 =
            m1.matrix[0, 0] * m2.matrix[0, 0] +
            m1.matrix[1, 0] * m2.matrix[0, 1] +
            m1.matrix[2, 0] * m2.matrix[0, 2];
        float a12 =
            m1.matrix[0, 0] * m2.matrix[1, 0] +
            m1.matrix[1, 0] * m2.matrix[1, 1] +
            m1.matrix[2, 0] * m2.matrix[1, 2];
        float a13 =
            m1.matrix[0, 0] * m2.matrix[2, 0] +
            m1.matrix[1, 0] * m2.matrix[2, 1] +
            m1.matrix[2, 0] * m2.matrix[2, 2];
        float a21 =
            m1.matrix[0, 1] * m2.matrix[0, 0] +
            m1.matrix[1, 1] * m2.matrix[0, 1] +
            m1.matrix[2, 1] * m2.matrix[0, 2];
        float a22 =
            m1.matrix[0, 1] * m2.matrix[1, 0] +
            m1.matrix[1, 1] * m2.matrix[1, 1] +
            m1.matrix[2, 1] * m2.matrix[1, 2];
        float a23 =
            m1.matrix[0, 1] * m2.matrix[2, 0] +
            m1.matrix[1, 1] * m2.matrix[2, 1] +
            m1.matrix[2, 1] * m2.matrix[2, 2];
        float a31 =
            m1.matrix[0, 2] * m2.matrix[0, 0] +
            m1.matrix[1, 2] * m2.matrix[0, 1] +
            m1.matrix[2, 2] * m2.matrix[0, 2];
        float a32 =
            m1.matrix[0, 2] * m2.matrix[1, 0] +
            m1.matrix[1, 2] * m2.matrix[1, 1] +
            m1.matrix[2, 2] * m2.matrix[1, 2];
        float a33 =
            m1.matrix[0, 2] * m2.matrix[2, 0] +
            m1.matrix[1, 2] * m2.matrix[2, 1] +
            m1.matrix[2, 2] * m2.matrix[2, 2];
        return new Matrix3x3(a11, a12, a13, a21, a22, a23, a31, a32, a33);
    }

    //FLOAT MULTIPLICATION
    public static Matrix3x3 operator *(Matrix3x3 m, float f)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                m.matrix[i, j] *= f;
            }
        }
        return m;
    }
    public static Matrix3x3 operator *(float f, Matrix3x3 m)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                m.matrix[i, j] *= f;
            }
        }
        return m;
    }

    //ToSTRING OVERRIDE
    public override string ToString()
    {
        string str = "[";
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                str += matrix[j, i].ToString();
                str += i * j == 4 ? "]" : ", ";
            }
        }
        return str;
    }
}

public static class ExtensionMethods
{
    public static Vector3 MultiplyPoint(this Matrix3x3 m, Vector3 point)
    {
        Vector3 newPoint;
        newPoint.x = m.matrix[0, 0] * point.x + m.matrix[1, 0] * point.y + m.matrix[2, 0];
        newPoint.y = m.matrix[0, 1] * point.x + m.matrix[1, 1] * point.y + m.matrix[2, 1];
        newPoint.z = point.z;
        return newPoint;
    }

    public static Vector3 MultiplyVector(this Matrix3x3 m, Vector3 vec)
    {
        Vector3 newVec;
        newVec.x = m.matrix[0, 0] * vec.x + m.matrix[1, 0] * vec.y;
        newVec.y = m.matrix[0, 1] * vec.x + m.matrix[1, 1] * vec.y;
        newVec.z = vec.z;
        return newVec;
    }

    public static Matrix3x3 Transpose(this Matrix3x3 m)
    {
        Matrix3x3 newM = new Matrix3x3();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                newM.matrix[i, j] = m.matrix[j, i];
            }
        }
        return newM;
    }

    public static Matrix3x3 Inverse(this Matrix3x3 m)
    {
        Matrix3x3 newM;
        float a11 = m.matrix[0, 0];
        float a12 = m.matrix[1, 0];
        float a13 = m.matrix[2, 0];
        float a21 = m.matrix[0, 1];
        float a22 = m.matrix[1, 1];
        float a23 = m.matrix[2, 1];
        float a31 = m.matrix[0, 2];
        float a32 = m.matrix[1, 2];
        float a33 = m.matrix[2, 2];

        float m11 = (a22 * a33) - (a23 * a32);
        float m12 = (a21 * a33) - (a23 * a31);
        float m13 = (a21 * a32) - (a22 * a31);
        float m21 = (a12 * a33) - (a13 * a32);
        float m22 = (a11 * a33) - (a13 * a31);
        float m23 = (a11 * a32) - (a12 * a31);
        float m31 = (a12 * a23) - (a13 * a22);
        float m32 = (a11 * a23) - (a13 * a21);
        float m33 = (a11 * a22) - (a12 * a21);

        newM = new Matrix3x3(m11, -m12, m13, -m21, m22, -m23, m31, -m32, m33);
        newM = newM.Transpose();
        float detM = (m * newM).matrix[0, 0];
        if (detM == 0)
        {
            Debug.Log("determinant is 0, no inverse could be found (original matrix has been returned)");
            return m;
        }
        else
        {
            detM = 1f / detM;
            newM = detM * newM;
            return newM;
        }
    }
}