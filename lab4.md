*Паршин Максим, 343*

## Лабораторная работа #4

### Вариант 10

*Численное решение интегрального уравнения Фредгольма 2-го рода (метод механических квадратур).*		

---

### Постановка задачи

Решить интегральное уравнение Фредгольма второго рода
$$
u(x) - \int_a^b H(x,y)u(y)\,\mathrm{d}y = f(x)
$$
методом механических квадратур с использованием составной квадратурной формулы средних прямоугольников.

### Исходные данные

$$
H(x, y) = 0.6 \cos(x, y^2)\\
f(x) = 0.6 - x\\
a = 0\\
b = 1\\
\epsilon = 0.000001
$$

### **Код**

Github: https://github.com/mxprshn/NumMethods/blob/4c5ada8877193e3b67092574252ec440b7b13195/numan/src/main/kotlin/laba4/lab4.kt

```kotlin
fun main() {
    // Исходные данные
    val H = { x: Double, y: Double -> 0.6 * cos(x * y.pow(2)) }
    val f = { x: Double -> x - 0.6 }
    val a = 0.0
    val b = 1.0

    val epsilon = 0.000001
    var nodesNumber = 1
    var max = Double.POSITIVE_INFINITY
    val result = mutableListOf<NResult>()

    // Пока не выполняется условие выхода, считаем приближенное решение уравнения с текущим количеством шагов,
    // в конце каждой итерации удваиваем его
    while (max >= epsilon) {
        val (u) = solveFredholm(nodesNumber, a, b, H, f)
        var nResult = NResult(
            nodesNumber,
            u(a),
            u((a + b) / 2),
            u(b),
            u,
            null
        )

        if (result.isNotEmpty()) {
            max = maxOf(
                abs(nResult.ua - result.last().ua),
                abs(nResult.um - result.last().um),
                abs(nResult.ub - result.last().ub)
            )
            nResult = nResult.copy(max = max)
        }

        result.add(nResult)
        nodesNumber *= 2
    }

    // Вывод результатов
    table {
        cellStyle {
            border = true
            alignment = TextAlignment.TopCenter
        }
        header { row("nodes", "u(a)","u(a + b / 2)", "u(b)", "max | u^2mn - u^mn |") }
        result.forEach {
            row(it.nodesNumber, it.ua.eFormat(), it.um.eFormat(), it.ub.eFormat(), it.max?.eFormat() ?: "--")
        }
    }.also { println(it) }
}

/**
 * Вспомогательный класс для хранения результатов итерации
 */
data class NResult(val nodesNumber: Int,
                   val ua: Double,
                   val um: Double,
                   val ub: Double,
                   val u: (Double) -> Double,
                   val max: Double?)

/**
 * Квардратурная формула средних прямоугольников
 */
fun rectangleMethod(nodesNumber: Int, a: Double, b: Double): IntegrationResult {
    require(b >= a)
    val h = (b - a) / nodesNumber
    val nodes = (1..nodesNumber).map { a + h / 2.0 + (it - 1) * h }
    return IntegrationResult(h, nodes)
}

data class IntegrationResult(val h: Double, val nodes: List<Double>)

fun solveFredholm(nodesNumber: Int,
                  a: Double,
                  b: Double,
                  H: (Double, Double) -> Double,
                  f: (Double) -> Double
): FredholmResult {
    val (h, nodes) = rectangleMethod(nodesNumber, a, b)
    val systemMatrix = Array(nodesNumber) { DoubleArray(nodesNumber) { 0.0 } }

    // Заполняем матрицу D
    for(j in 0 until nodesNumber) {
        for(k in 0 until nodesNumber) {
            systemMatrix[j][k] = kronecker(j, k) - h * H(nodes[j], nodes[k])
        }
    }
    val constVector = nodes.map { f(it) }.toDoubleArray().toVector()
    val solver = LUDecomposition(systemMatrix.toMatrix()).solver
    val solution = solver.solve(constVector)
    return FredholmResult { x: Double ->
        // Решение
        f(x) + h * nodes.mapIndexed { i, xi -> H(x, xi) * solution[i] }.sum()
    }
}

data class FredholmResult(val u: (Double) -> Double)
```

### Результаты

```
┌─────┬──────────────┬──────────────┬─────────────┬────────────────────┐
│nodes│     u(a)     │ u(a + b / 2) │    u(b)     │max | u^2mn - u^mn |│
├─────┼──────────────┼──────────────┼─────────────┼────────────────────┤
│  1  │-7.4826478e-01│-2.4710797e-01│2.5634441e-01│         --         │
├─────┼──────────────┼──────────────┼─────────────┼────────────────────┤
│  2  │-7.4986906e-01│-2.4979924e-01│2.5041041e-01│   5.9339967e-03    │
├─────┼──────────────┼──────────────┼─────────────┼────────────────────┤
│  4  │-7.5155154e-01│-2.5235894e-01│2.4539459e-01│   5.0158267e-03    │
├─────┼──────────────┼──────────────┼─────────────┼────────────────────┤
│  8  │-7.5204941e-01│-2.5310797e-01│2.4397469e-01│   1.4198953e-03    │
├─────┼──────────────┼──────────────┼─────────────┼────────────────────┤
│ 16  │-7.5217838e-01│-2.5330160e-01│2.4361079e-01│   3.6389787e-04    │
├─────┼──────────────┼──────────────┼─────────────┼────────────────────┤
│ 32  │-7.5221090e-01│-2.5335040e-01│2.4351928e-01│   9.1508937e-05    │
├─────┼──────────────┼──────────────┼─────────────┼────────────────────┤
│ 64  │-7.5221905e-01│-2.5336263e-01│2.4349637e-01│   2.2910274e-05    │
├─────┼──────────────┼──────────────┼─────────────┼────────────────────┤
│ 128 │-7.5222109e-01│-2.5336568e-01│2.4349064e-01│   5.7296279e-06    │
├─────┼──────────────┼──────────────┼─────────────┼────────────────────┤
│ 256 │-7.5222160e-01│-2.5336645e-01│2.4348921e-01│   1.4325356e-06    │
├─────┼──────────────┼──────────────┼─────────────┼────────────────────┤
│ 512 │-7.5222172e-01│-2.5336664e-01│2.4348885e-01│   3.5814193e-07    │
└─────┴──────────────┴──────────────┴─────────────┴────────────────────┘

```

При числе узлов интегрирования, равном 512, было выполнено условие остановки по достижении заданной точности:
$$
\max _{i=1,2,3}\left|u^{2 m n}\left(x_{i}\right)-u^{m n}\left(x_{i}\right)\right|<\varepsilon, \quad x_{1}=a, \ x_{2}=(a+b) / 2, \ x_{3}=b
$$
