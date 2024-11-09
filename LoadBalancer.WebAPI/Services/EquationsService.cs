using System.Threading;

namespace LoadBalancer.WebAPI.Services
{
    public class EquationsService
    {
        public delegate void ProgressUpdateHandler(int progress);
        public event ProgressUpdateHandler OnProgressUpdate;

        public double[] Solve(int n, CancellationToken cancellationToken)
        {
            double[][] A = MatrixService.GenerateMatrix(n);
            double[] b = MatrixService.GenerateVector(n);

            for (int i = 0; i < n; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException();
                }

                if (Math.Abs(A[i][i]) < 1e-10)
                {
                    throw new InvalidOperationException("Matrix A is singular or nearly singular.");
                }

                for (int j = i + 1; j < n; j++)
                {
                    double factor = A[j][i] / A[i][i];
                    for (int k = i; k < n; k++)
                    {
                        A[j][k] -= factor * A[i][k];
                    }
                    b[j] -= factor * b[i];
                }

                int progress = (int)((i + 1) / (double)n * 100);
                OnProgressUpdate?.Invoke(progress);
            }

            double[] x = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                x[i] = b[i] / A[i][i];
                for (int j = i + 1; j < n; j++)
                {
                    x[i] -= A[i][j] * x[j] / A[i][i];
                }
            }

            OnProgressUpdate?.Invoke(100);
            return x;
        }

    }
}
