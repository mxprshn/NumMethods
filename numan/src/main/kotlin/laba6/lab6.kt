package laba6

import com.jakewharton.picnic.TextAlignment
import com.jakewharton.picnic.table
import eFormat
import get
import org.apache.commons.math3.analysis.UnivariateFunction
import org.apache.commons.math3.analysis.integration.BaseAbstractUnivariateIntegrator
import org.apache.commons.math3.analysis.integration.IterativeLegendreGaussIntegrator
import org.apache.commons.math3.analysis.polynomials.PolynomialFunction
import org.apache.commons.math3.analysis.polynomials.PolynomialsUtils
import org.apache.commons.math3.linear.LUDecomposition
import org.apache.commons.math3.linear.RealMatrix
import set
import zeroMatrix
import zeroVector
import kotlin.math.*

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