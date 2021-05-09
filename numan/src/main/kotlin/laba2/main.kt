package laba2

import com.jakewharton.picnic.TextAlignment
import com.jakewharton.picnic.table
import eFormat
import format
import getLastRow
import minus
import toMatrix
import toTable
import toVector

fun main(args: Array<String>) {
    val approximationIndex = 8
    val upperRelaxationQStep = 0.1

    Lab2(Data.lab2Variant10Matrix.toMatrix(), Data.lab2Variant10Constants.toVector()).apply {
        println("A matrix:")
        println(sourceMatrix.toTable())

        println("b vector:")
        println(sourceConstVector.toTable())

        println("Eigenvalues: ${sourceEigenValues.map { it.format() }.joinToString()}")

        println("Exact solution:")
        println(exactSolution.toTable())

        println("Optimal parameter alpha: ${optimalAlpha.eFormat()}")

        println("H matrix:")
        println(alphaOptimalH.toTable())

        println("g vector:")
        println(alphaOptimalG.toTable())

        println("Norm of H: ${alphaOptimalH.norm.eFormat()}")

        val iterSolutions = findSolutionsWithIterMethod(approximationIndex)
        val iterSolution = iterSolutions.getLastRow()
        println("Solution found with iter method:")
        println(iterSolution.toTable())
        val posteriorEstimation = findIterPosteriorEstimation(iterSolutions)
        println("Posterior error estimation: ${posteriorEstimation.eFormat()}")
        val iterActualError = (iterSolution - exactSolution).norm
        println("Actual iter method error: ${iterActualError.eFormat()}")

        val refinedIterSolution = refineIterSolutionWithLyusternikMethod(iterSolutions)
        println("Refined iter solution with Lyusternik method:")
        println(refinedIterSolution.toTable())
        val refinedIterActualError = (refinedIterSolution - exactSolution).norm
        println("Actual refined iter method error: ${refinedIterActualError.eFormat()}")

        val seidelSolutions = findSolutionsWithSeidelMethod(approximationIndex)
        val seidelSolution = seidelSolutions.getLastRow()
        println("Solution found with Seidel method:")
        println(seidelSolution.toTable())
        val seidelActualError = (seidelSolution - exactSolution).norm
        println("Actual seidel method error: ${seidelActualError.eFormat()}")

        println("Seidel transformation matrix:")
        println(seidelTransformationMatrix.toTable())
        val refinedSeidelSolution = refineSeidelSolutionWithLyusternikMethod(seidelSolutions)
        println("Refined Seidel solution with Lyusternik method:")
        println(refinedSeidelSolution.toTable())
        val refinedSeidelActualError = (refinedSeidelSolution - exactSolution).norm
        println("Actual refined Seidel method error: ${refinedSeidelActualError.eFormat()}")

        val upperRelaxationWithOptimalSolution =
            findSolutionsWithUpperRelaxationMethod(optimalQForUpperRelaxationMethod, approximationIndex).getLastRow()
        println("Optimal upper relaxation solution:")
        println(upperRelaxationWithOptimalSolution.toTable())
        val upperRelaxationWithOptimalSolutionActualError = (upperRelaxationWithOptimalSolution - exactSolution).norm
        println("Actual optimal upper relaxation method error: ${upperRelaxationWithOptimalSolutionActualError.eFormat()}")

        val upperRelaxationWithOptimalSolutionMinus =
            findSolutionsWithUpperRelaxationMethod(
                optimalQForUpperRelaxationMethod - upperRelaxationQStep,
                approximationIndex
            ).getLastRow()
        println("Optimal - ${upperRelaxationQStep.format(2)} upper relaxation solution:")
        println(upperRelaxationWithOptimalSolutionMinus.toTable())
        val upperRelaxationWithOptimalSolutionMinusActualError = (upperRelaxationWithOptimalSolutionMinus - exactSolution).norm
        println("Actual optimal - ${upperRelaxationQStep.format(2)} " +
                "upper relaxation method error: ${upperRelaxationWithOptimalSolutionMinusActualError.eFormat()}")

        val upperRelaxationWithOptimalSolutionPlus =
            findSolutionsWithUpperRelaxationMethod(
                optimalQForUpperRelaxationMethod + upperRelaxationQStep,
                approximationIndex
            ).getLastRow()
        println("Optimal + ${upperRelaxationQStep.format(2)} upper relaxation solution:")
        println(upperRelaxationWithOptimalSolutionPlus.toTable())
        val upperRelaxationWithOptimalSolutionPlusActualError = (upperRelaxationWithOptimalSolutionPlus - exactSolution).norm
        println("Actual optimal + ${upperRelaxationQStep.format(2)} " +
                "upper relaxation method error: ${upperRelaxationWithOptimalSolutionPlusActualError.eFormat()}")

        table {
            cellStyle {
                border = true
                alignment = TextAlignment.TopCenter
            }
            header {
                row("Method", "Solution", "Actual error")
            }
            row("Exact method", exactSolution.toTable(), 0)
            row("Iter method", iterSolution.toTable(), iterActualError.eFormat())
            row("Refined iter method", refinedIterSolution.toTable(), refinedIterActualError.eFormat())
            row("Seidel method", seidelSolution.toTable(), seidelActualError.eFormat())
            row("Refined Seidel method", refinedSeidelSolution.toTable(), refinedSeidelActualError.eFormat())
            row("Optimal upper relaxation", upperRelaxationWithOptimalSolution.toTable(), upperRelaxationWithOptimalSolutionActualError.eFormat())
            row(
                "Optimal - ${upperRelaxationQStep.format(2)} upper relaxation",
                upperRelaxationWithOptimalSolutionMinus.toTable(),
                upperRelaxationWithOptimalSolutionMinusActualError.eFormat()
            )
            row(
                "Optimal + ${upperRelaxationQStep.format(2)} upper relaxation",
                upperRelaxationWithOptimalSolutionPlus.toTable(),
                upperRelaxationWithOptimalSolutionPlusActualError.eFormat()
            )
        }.also { println(it) }
    }
}