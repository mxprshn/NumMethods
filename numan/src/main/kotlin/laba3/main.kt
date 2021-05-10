package laba3

import com.jakewharton.picnic.TextAlignment
import com.jakewharton.picnic.table
import eFormat
import format
import minus
import times
import toMatrix
import toTable
import toVector
import kotlin.math.sqrt

fun main(args: Array<String>) {
    val emergencyIterationsThreshold = 100
    val powerMethodEpsilon = 0.001
    val dotProductMethodEpsilon = 0.000001

    Lab3(
        Data.lab3Variant10Matrix.toMatrix(),
        emergencyIterationsThreshold,
        Data.lab3Variant10InitialVector.toVector()
    ).apply {
        println("Source matrix:")
        println(matrix.toTable())
        println("Exact max abs eigenvalue: ${exactMaxAbsEigenValue.format()}")
        println("lambda_2/lambda_1: ${lambda2OverLambda1.format()}")

        println("Exact normalized eigenvector:")
        println(exactNormalizedEigenvector.toTable())

        val powerMethodResult = findMaxEigenvalueWithPowerMethod(powerMethodEpsilon)
        val dotProductMethodResult = findMaxEigenvalueWithDotProductMethod(dotProductMethodEpsilon)

        fun printTable(data: List<Lab3.IterationResult>) {
            table {
                cellStyle {
                    border = true
                    alignment = TextAlignment.TopCenter
                }
                header { row("k", "λ^k", "λ^k - λ^(k-1)", "λ^k - λ^*", "|| A x^k - λ^k x^k ||", "error est post", "eigenvector") }
                data.zipWithNext { prev, current -> prev to current }
                        .forEachIndexed { i, (prev, current) ->
                            row {
                                cell(i + 1)
                                cell(current.lambda.format())
                                cell((current.lambda - prev.lambda).eFormat())
                                cell((current.lambda - exactMaxAbsEigenValue).eFormat())
                                cell((matrix * current.eigenvector - current.lambda * current.eigenvector).norm.eFormat())
                                cell(current.posteriorEstimation.eFormat())
                                cell(current.eigenvector.toTable())
                            }
                        }
            }.also { println(it) }
        }

        println("Power method result:")
        printTable(powerMethodResult)

        println("Dot product method result:")
        printTable(dotProductMethodResult)
    }
}