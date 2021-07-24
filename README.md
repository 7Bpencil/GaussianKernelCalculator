# GaussianKernelCalculator

My take on gaussian kernels.  
Credits: theomader (base calculator) and Steve M (bilinear version) at https://dev.theomader.com/gaussian-kernel-calculator/  

Example, kernel size = 5, sigma = 1:  
```
static void Main(string[] args)
{
    var kernelSize = 5;
    var sigma = 1d;

    GaussianKernelsHelper.GetKernel(sigma, kernelSize, out var weights, out var samplePositions);
    GaussianKernelsHelper.GetKernelBilinear(sigma, kernelSize, out var weightsBilinear, out var samplePositionsBilinear);

    Console.Write("original:\nweights: ");
    foreach (var w in weights) Console.Write($"{Math.Round(w, 6)} ");
    Console.Write("\nsample positions: ");
    foreach (var p in samplePositions) Console.Write($"{Math.Round(p, 6)} ");
    Console.Write("\n\nbilinear:\nweights: ");
    foreach (var w in weightsBilinear) Console.Write($"{Math.Round(w, 6)} ");
    Console.Write("\nsample positions: ");
    foreach (var p in samplePositionsBilinear) Console.Write($"{Math.Round(p, 6)} ");
}
```
Output:  
```
original:
weights: 0.06136 0.24477 0.38774 0.24477 0.06136
sample positions: -2 -1 0 1 2

bilinear:
weights: 0.30613 0.38774 0.30613
sample positions: -1.200436 0 1.200436
```

Bilinear version works only for kernels with size of 5, 9, 13 ... or generally: (kernelSize - 1) % 4 == 0
