import org.jetbrains.kotlin.gradle.tasks.KotlinCompile

plugins {
    kotlin("jvm") version "1.4.10"
    application
}
group = "me.mxprshn"
version = "1.0-SNAPSHOT"

repositories {
    mavenCentral()
}

tasks.withType<KotlinCompile>() {
    kotlinOptions.jvmTarget = "1.8"
}
application {
    mainClassName = "MainKt"
}
dependencies {
    val mathVersion = "3.6.1"
    implementation("org.apache.commons:commons-math3:$mathVersion")

    val picnicVersion = "0.5.0"
    implementation("com.jakewharton.picnic:picnic:$picnicVersion")
}