*Паршин Максим, 343*

## Лабораторная работа #6

### Вариант 10

*Проекционные методы решения краевой задачи для дифференциального уравнения второго порядка*

---

### Постановка задачи

Найти решение граничной задачи 

$$
-\frac{5-x}{7-3 x} u^{\prime \prime}-\frac{1-x}{2} u^{\prime}+\left(1+\frac{1}{2} \sin (x)\right) u=\frac{1}{2}+\frac{x}{2}, u^{\prime}(-1)=2 u^{\prime}(1)+3 u(1)=0
$$

методом Галеркина и методом коллокации. Использовать следующую координатную систему
$$
W_{1}, W_{2},W_{i}(x)=\left(1-x^{2}\right)^{2} P_{i-3}^{(2,2)}(x), i=3,4, \ldots
$$
где $W_1$ и $W_2$ — полиномы не выше третьей степени, удовлетворяющие граничным условиям, $P_{i-3}^{(2,2)}$ — многочлены Якоби

### Исходные данные

$$
\begin{array}
&W_1 = x^2 + 2x - \frac{17}3\\
W_2 = x^3 - 3x + 2
\end{array}
$$

### **Код**

Github: https://github.com/mxprshn/NumMethods/blob/master/numan/src/main/kotlin/laba6/lab6.kt

```kotlin
data class FunctionWithDerivatives(val f: (Double) -> Double, val dfdx: (Double) -> Double, val dfdx2: (Double) -> Double)

/**
 * Дифференциальный оператор из условия
 */
val L: (FunctionWithDerivatives) -> ((Double) -> Double) = { (f, dfdx, dfdx2) ->
    { x ->
        -(5 - x) / (7 - 3 * x) * dfdx2(x) - (1 - x) / 2 * dfdx(x) + (1 + 0.5 * sin(x)) * f(x)
    }
}

val f = { x: Double -> 0.5 + x / 2 }

val nRange = (3..10)

fun main() {
    table {
        cellStyle {
            border = true
            alignment = TextAlignment.TopCenter
        }
        row {
            cell("n") { rowSpan = 2 }
            cell("μ(A) Galerkin") { rowSpan = 2 }
            cell("μ(A) collocation") { rowSpan = 2 }
            cell("y^n(x) Galerkin") { columnSpan = 3 }
            cell("y^n(x) collocation") { columnSpan = 3 }
            cell("diff") { columnSpan = 3 }
        }

        row("-0.5", "0", "0.5", "-0.5", "0", "0.5", "-0.5", "0", "0.5")

        nRange.forEach { n ->
            val galerkinSolution = solveWithGalerkin(n, L, f)
            val collocationSolution = solveWithCollocation(n, L, f)
            row{
                cell(n)
                cell(galerkinSolution.matrix.cond().eFormat())
                cell(collocationSolution.matrix.cond().eFormat())

                cell(galerkinSolution.y(-0.5).eFormat())
                cell(galerkinSolution.y(0.0).eFormat())
                cell(galerkinSolution.y(0.5).eFormat())

                cell(collocationSolution.y(-0.5).eFormat())
                cell(collocationSolution.y(0.0).eFormat())
                cell(collocationSolution.y(0.5).eFormat())

                cell(abs(galerkinSolution.y(-0.5) - collocationSolution.y(-0.5)).eFormat())
                cell(abs(galerkinSolution.y(0.0) - collocationSolution.y(0.0)).eFormat())
                cell(abs(galerkinSolution.y(0.5) - collocationSolution.y(0.5)).eFormat())
            }
        }
    }.also { println(it) }
}

/**
 * Считаем координатные функции, причем первые две заданы отдельно
 */
fun getCoordinateFunctions(n: Int): List<FunctionWithDerivatives> {
    val from2 = (2 until n)
        .map { PolynomialsUtils.createJacobiPolynomial(it - 2, 2, 2) }
        .map { it.multiply(PolynomialFunction(doubleArrayOf(1.0, 0.0, -2.0, 0.0, 1.0))) }
        .map { pf ->
            FunctionWithDerivatives(
                f = { x -> pf.value(x) },
                dfdx = { x -> pf.polynomialDerivative().value(x) },
                dfdx2 = { x -> pf.polynomialDerivative().polynomialDerivative().value(x) }
            )
        }
    val before2 = listOf(
        FunctionWithDerivatives(
            f = { x -> x.pow(2) + 2 * x - 17.0 / 3.0 },
            dfdx = { x -> 2 * (x + 1) },
            dfdx2 = { x -> 2.0 }
        ),
        FunctionWithDerivatives(
            f = { x -> x.pow(3) - 3 * x + 2 },
            dfdx = { x -> 3 * (x.pow(2) - 1) },
            dfdx2 = { x -> 6 * x }
        )
    )

    return before2 + from2
}

data class Solution(val n: Int, val matrix: RealMatrix, val y: (Double) -> Double)

/**
 * Решение методом Галеркина
 */
fun solveWithGalerkin(n: Int,
                      L: (FunctionWithDerivatives) -> ((Double) -> Double),
                      f: (Double) -> Double
): Solution {
    val coordinateFunctions = getCoordinateFunctions(n)
    val systemMatrix = zeroMatrix(n)
    val constVector = zeroVector(n)
    for (i in 0 until n) {
        constVector[i] = f dot coordinateFunctions[i].f
        for (j in 0 until n) {
            systemMatrix[i, j] = L(coordinateFunctions[j]) dot coordinateFunctions[i].f
        }
    }
    val solver = LUDecomposition(systemMatrix).solver
    val solutionCoefficients = solver.solve(constVector)
    val solution = { x: Double ->
        var sum = 0.0
        for (i in 0 until n) {
            sum += solutionCoefficients[i] * coordinateFunctions[i].f(x)
        }
        sum
    }
    return Solution(n, systemMatrix, solution)
}

/**
 * Решение методом коллокации
 */
fun solveWithCollocation(n: Int,
                         L: (FunctionWithDerivatives) -> ((Double) -> Double),
                         f: (Double) -> Double
): Solution {
    val coordinateFunctions = getCoordinateFunctions(n)
    val chebyshevNodes = (0 until n).map { cos((2.0 * (it + 1.0) - 1.0) / (2.0 * n) * PI) }
    val systemMatrix = zeroMatrix(n)
    val constVector = zeroVector(n)
    for (i in 0 until n) { 
        constVector[i] = f(chebyshevNodes[i])
        for (j in 0 until n) {
            systemMatrix[i, j] = L(coordinateFunctions[j])(chebyshevNodes[i])
        }
    }
    val solver = LUDecomposition(systemMatrix).solver
    val solutionCoefficients = solver.solve(constVector)
    val solution = { x: Double ->
        var sum = 0.0
        for (i in 0 until n) {
            sum += solutionCoefficients[i] * coordinateFunctions[i].f(x)
        }
        sum
    }
    return Solution(n, systemMatrix, solution)
}

/**
 * Число обусловленности
 */
fun RealMatrix.cond(): Double {
    val solver = LUDecomposition(this).solver
    return this.norm * solver.inverse.norm
}

/**
 * Скалярное произведение
 */
infix fun ((Double) -> Double).dot(other: (Double) -> Double): Double {
    val integrator = IterativeLegendreGaussIntegrator(
        10,
        BaseAbstractUnivariateIntegrator.DEFAULT_RELATIVE_ACCURACY,
        BaseAbstractUnivariateIntegrator.DEFAULT_ABSOLUTE_ACCURACY,
        BaseAbstractUnivariateIntegrator.DEFAULT_MIN_ITERATIONS_COUNT,
        BaseAbstractUnivariateIntegrator.DEFAULT_MAX_ITERATIONS_COUNT
    )
    val function = UnivariateFunction { x -> this(x) * other(x) }
    return integrator.integrate(10000000, function, -1.0, 1.0)
}
```

### Результаты

В столбце `diff` содержится модуль разности между значениями решений методом Галеркина и методом коллокации

```
┌──┬─────────────┬────────────────┐
│n │μ(A) Galerkin│μ(A) collocation│
├──┼─────────────┼────────────────┤
│3 │7.4546871e+01│ 3.9712236e+00  │
├──┼─────────────┼────────────────┤
│4 │7.5541104e+01│ 8.9261190e+00  │
├──┼─────────────┼────────────────┤
│5 │1.1159155e+02│ 1.5879944e+01  │
├──┼─────────────┼────────────────┤
│6 │1.1215431e+02│ 2.8531155e+01  │
├──┼─────────────┼────────────────┤
│7 │1.3362114e+02│ 4.0093058e+01  │
├──┼─────────────┼────────────────┤
│8 │1.3369261e+02│ 5.5209805e+01  │
├──┼─────────────┼────────────────┤
│9 │1.4875915e+02│ 7.0254499e+01  │
├──┼─────────────┼────────────────┤
│10│1.4872326e+02│ 8.9472013e+01  │
└──┴─────────────┴────────────────┘

┌──┬─────────────────────────────────────────┬─────────────────────────────────────────┐
│n │             y^n(x) Galerkin             │           y^n(x) collocation            │
│  ├─────────────┬─────────────┬─────────────┼─────────────┬─────────────┬─────────────┤
│  │    -0.5     │      0      │     0.5     │    -0.5     │      0      │     0.5     │
├──┼─────────────┼─────────────┼─────────────┼─────────────┼─────────────┼─────────────┤
│3 │3.5837731e-01│3.7983652e-01│3.5664025e-01│3.5275815e-01│3.8083244e-01│3.6247900e-01│
├──┼─────────────┼─────────────┼─────────────┼─────────────┼─────────────┼─────────────┤
│4 │3.6124578e-01│3.8019265e-01│3.5479463e-01│3.6122009e-01│3.8071966e-01│3.5477747e-01│
├──┼─────────────┼─────────────┼─────────────┼─────────────┼─────────────┼─────────────┤
│5 │3.6152703e-01│3.8001234e-01│3.5499422e-01│3.6161276e-01│3.8003391e-01│3.5492136e-01│
├──┼─────────────┼─────────────┼─────────────┼─────────────┼─────────────┼─────────────┤
│6 │3.6153801e-01│3.8001296e-01│3.5499603e-01│3.6154822e-01│3.7998166e-01│3.5500686e-01│
├──┼─────────────┼─────────────┼─────────────┼─────────────┼─────────────┼─────────────┤
│7 │3.6153342e-01│3.8002555e-01│3.5498945e-01│3.6153277e-01│3.8002477e-01│3.5499008e-01│
├──┼─────────────┼─────────────┼─────────────┼─────────────┼─────────────┼─────────────┤
│8 │3.6153282e-01│3.8002572e-01│3.5499014e-01│3.6153202e-01│3.8002623e-01│3.5498952e-01│
├──┼─────────────┼─────────────┼─────────────┼─────────────┼─────────────┼─────────────┤
│9 │3.6153287e-01│3.8002548e-01│3.5499018e-01│3.6153279e-01│3.8002548e-01│3.5499025e-01│
├──┼─────────────┼─────────────┼─────────────┼─────────────┼─────────────┼─────────────┤
│10│3.6153291e-01│3.8002548e-01│3.5499016e-01│3.6153290e-01│3.8002546e-01│3.5499015e-01│
└──┴─────────────┴─────────────┴─────────────┴─────────────┴─────────────┴─────────────┘

┌──┬─────────────────────────────────────────┐
│n │                  diff                   │
│  ├─────────────┬─────────────┬─────────────┤
│  │    -0.5     │      0      │     0.5     │
├──┼─────────────┼─────────────┼─────────────┤
│3 │5.6191647e-03│9.9591392e-04│5.8387469e-03│
├──┼─────────────┼─────────────┼─────────────┤
│4 │2.5697587e-05│5.2701352e-04│1.7156851e-05│
├──┼─────────────┼─────────────┼─────────────┤
│5 │8.5728331e-05│2.1576159e-05│7.2866622e-05│
├──┼─────────────┼─────────────┼─────────────┤
│6 │1.0211748e-05│3.1297989e-05│1.0830661e-05│
├──┼─────────────┼─────────────┼─────────────┤
│7 │6.5118830e-07│7.8640280e-07│6.2441818e-07│
├──┼─────────────┼─────────────┼─────────────┤
│8 │8.0218694e-07│5.1501652e-07│6.1658741e-07│
├──┼─────────────┼─────────────┼─────────────┤
│9 │8.0099590e-08│2.8585531e-09│6.1493391e-08│
├──┼─────────────┼─────────────┼─────────────┤
│10│4.0557204e-09│1.5946582e-08│1.8636118e-09│
└──┴─────────────┴─────────────┴─────────────┘

```

Таким образом, значения решений, полученных данными методами, сходятся друг к другу с увеличением числа функций в координатной системе
