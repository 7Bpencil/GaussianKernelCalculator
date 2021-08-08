# GaussianKernelCalculator

My take on gaussian kernels.  

### Credits:   
theomader (base calculator) and Steve M (bilinear version)   
at https://dev.theomader.com/gaussian-kernel-calculator/  

### Bilinear version?  
We can reduce size of a kernel by calculating two samples for the cost of one using bilinear filtering.  
Basically we can get the same result with smaller kernel or better result with kernel of the same size.  
It works only for kernels with sizes of 5, 9, 13 ... or generally: N * 4 + 1 for N > 0.  
New bilinear kernel size = (kernelSize + 1) / 2  

### Example:  
```
using GKH = SevenBoldPencil.MathTools.GaussianKernelsHelper;
static void Main(string[] args)
{
    var kernelSize = 5;
    var sigma = 1d;

    GKH.GetKernel(sigma, kernelSize, out var weights, out var samplePositions);
    GKH.GetKernelBilinear(sigma, kernelSize, out var weightsBilinear, out var samplePositionsBilinear);
    GKH.GetKernelBilinearSameSize(sigma, kernelSize, out var weightsBilinearSS, out var samplePositionsBilinearSS);

    PrintKernel("original", weights, samplePositions);
    PrintKernel("\n\nbilinear", weightsBilinear, samplePositionsBilinear);
    PrintKernel("\n\nbilinear same size", weightsBilinearSS, samplePositionsBilinearSS);
}

static void PrintKernel(string name, double[] weights, double[] samplePositions)
{
    Console.Write($"{name}:\nweights: ");
    foreach (var w in weights) Console.Write($"{Math.Round(w, 6)} ");
    Console.Write("\nsample positions: ");
    foreach (var p in samplePositions) Console.Write($"{Math.Round(p, 6)} ");
}
```
### Output:  
```
original:
weights: 0.06136 0.24477 0.38774 0.24477 0.06136
sample positions: -2 -1 0 1 2

bilinear:
weights: 0.30613 0.38774 0.30613
sample positions: -1.200436 0 1.200436

bilinear same size:
weights: 0.006206 0.30233 0.382928 0.30233 0.006206
sample positions: -3.036935 -1.200436 0 1.200436 3.036935
```
