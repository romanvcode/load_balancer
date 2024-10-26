namespace LoadBalancer.WebAPI.Services
{
    public class EquationsService
    {
        public double[] Solve(double[,] A, double[] b)
        {
            int n = b.Length;

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (Math.Abs(A[j, i]) > Math.Abs(A[i, i]))
                    {
                        for (int k = 0; k < n; k++)
                        {
                            double temp = A[i, k];
                            A[i, k] = A[j, k];
                            A[j, k] = temp;
                        }
                        double tempB = b[i];
                        b[i] = b[j];
                        b[j] = tempB;
                    }
                }

                for (int j = i + 1; j < n; j++)
                {
                    double factor = A[j, i] / A[i, i];
                    for (int k = i; k < n; k++)
                    {
                        A[j, k] -= factor * A[i, k];
                    }
                    b[j] -= factor * b[i];
                }
            }

            double[] x = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                x[i] = b[i] / A[i, i];
                for (int j = i + 1; j < n; j++)
                {
                    x[i] -= A[i, j] * x[j] / A[i, i];
                }
            }
            return x;
        }
    }
}
