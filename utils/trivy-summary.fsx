#r "nuget: FSharp.Data, 6.6.0"

open FSharp.Data
open System.Collections.Generic

[<Literal>]
let ResolutionFolder = __SOURCE_DIRECTORY__

// Sample from https://trivy.dev/latest/docs/configuration/reporting/#json
[<Literal>]
let sample =
    """{
  "SchemaVersion": 2,
  "CreatedAt": "2024-12-26T21:58:15.943876+05:30",
  "ArtifactName": "alpine:latest",
  "ArtifactType": "container_image",
  "Metadata": {
    "OS": {
      "Family": "alpine",
      "Name": "3.20.3"
    },
    "ImageID": "sha256:511a44083d3a23416fadc62847c45d14c25cbace86e7a72b2b350436978a0450",
    "DiffIDs": [
      "sha256:651d9022c23486dfbd396c13db293af6845731cbd098a5f5606db4bc9f5573e8"
    ],
    "RepoTags": [
      "alpine:latest"
    ],
    "RepoDigests": [
      "alpine@sha256:1e42bbe2508154c9126d48c2b8a75420c3544343bf86fd041fb7527e017a4b4a"
    ],
    "ImageConfig": {
      "architecture": "arm64",
      "created": "2024-09-06T12:05:36Z",
      "history": [
        {
          "created": "2024-09-06T12:05:36Z",
          "created_by": "ADD alpine-minirootfs-3.20.3-aarch64.tar.gz / # buildkit",
          "comment": "buildkit.dockerfile.v0"
        },
        {
          "created": "2024-09-06T12:05:36Z",
          "created_by": "CMD [\"/bin/sh\"]",
          "comment": "buildkit.dockerfile.v0",
          "empty_layer": true
        }
      ],
      "os": "linux",
      "rootfs": {
        "type": "layers",
        "diff_ids": [
          "sha256:651d9022c23486dfbd396c13db293af6845731cbd098a5f5606db4bc9f5573e8"
        ]
      },
      "config": {
        "Cmd": [
          "/bin/sh"
        ],
        "Env": [
          "PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin"
        ],
        "WorkingDir": "/",
        "ArgsEscaped": true
      }
    }
  },
  "Results": [
    {
      "Target": "alpine:latest (alpine 3.20.3)",
      "Class": "os-pkgs",
      "Type": "alpine",
      "Vulnerabilities": [
        {
          "VulnerabilityID": "CVE-2024-9143",
          "PkgID": "libcrypto3@3.3.2-r0",
          "PkgName": "libcrypto3",
          "PkgIdentifier": {
            "PURL": "pkg:apk/alpine/libcrypto3@3.3.2-r0?arch=aarch64\u0026distro=3.20.3",
            "UID": "f705555b49cd2259"
          },
          "InstalledVersion": "3.3.2-r0",
          "FixedVersion": "3.3.2-r1",
          "Status": "fixed",
          "Layer": {
            "DiffID": "sha256:651d9022c23486dfbd396c13db293af6845731cbd098a5f5606db4bc9f5573e8"
          },
          "PrimaryURL": "https://avd.aquasec.com/nvd/cve-2024-9143",
          "DataSource": {
            "ID": "alpine",
            "Name": "Alpine Secdb",
            "URL": "https://secdb.alpinelinux.org/"
          },
          "Title": "openssl: Low-level invalid GF(2^m) parameters lead to OOB memory access",
          "Description": "Issue summary: Use of the low-level GF(2^m) elliptic curve APIs with untrusted\nexplicit values for the field polynomial can lead to out-of-bounds memory reads\nor writes.\n\nImpact summary: Out of bound memory writes can lead to an application crash or\neven a possibility of a remote code execution, however, in all the protocols\ninvolving Elliptic Curve Cryptography that we're aware of, either only \"named\ncurves\" are supported, or, if explicit curve parameters are supported, they\nspecify an X9.62 encoding of binary (GF(2^m)) curves that can't represent\nproblematic input values. Thus the likelihood of existence of a vulnerable\napplication is low.\n\nIn particular, the X9.62 encoding is used for ECC keys in X.509 certificates,\nso problematic inputs cannot occur in the context of processing X.509\ncertificates.  Any problematic use-cases would have to be using an \"exotic\"\ncurve encoding.\n\nThe affected APIs include: EC_GROUP_new_curve_GF2m(), EC_GROUP_new_from_params(),\nand various supporting BN_GF2m_*() functions.\n\nApplications working with \"exotic\" explicit binary (GF(2^m)) curve parameters,\nthat make it possible to represent invalid field polynomials with a zero\nconstant term, via the above or similar APIs, may terminate abruptly as a\nresult of reading or writing outside of array bounds.  Remote code execution\ncannot easily be ruled out.\n\nThe FIPS modules in 3.3, 3.2, 3.1 and 3.0 are not affected by this issue.",
          "Severity": "LOW",
          "CweIDs": [
            "CWE-787"
          ],
          "VendorSeverity": {
            "amazon": 3,
            "redhat": 1,
            "ubuntu": 1
          },
          "CVSS": {
            "redhat": {
              "V3Vector": "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:N/I:N/A:L",
              "V3Score": 3.7
            }
          },
          "References": [
            "https://access.redhat.com/security/cve/CVE-2024-9143",
            "https://github.com/openssl/openssl/commit/72ae83ad214d2eef262461365a1975707f862712",
            "https://github.com/openssl/openssl/commit/bc7e04d7c8d509fb78fc0e285aa948fb0da04700",
            "https://github.com/openssl/openssl/commit/c0d3e4d32d2805f49bec30547f225bc4d092e1f4",
            "https://github.com/openssl/openssl/commit/fdf6723362ca51bd883295efe206cb5b1cfa5154",
            "https://github.openssl.org/openssl/extended-releases/commit/8efc0cbaa8ebba8e116f7b81a876a4123594d86a",
            "https://github.openssl.org/openssl/extended-releases/commit/9d576994cec2b7aa37a91740ea7e680810957e41",
            "https://nvd.nist.gov/vuln/detail/CVE-2024-9143",
            "https://openssl-library.org/news/secadv/20241016.txt",
            "https://www.cve.org/CVERecord?id=CVE-2024-9143"
          ],
          "PublishedDate": "2024-10-16T17:15:18.13Z",
          "LastModifiedDate": "2024-11-08T16:35:21.58Z"
        },
        {
          "VulnerabilityID": "CVE-2024-9143",
          "PkgID": "libssl3@3.3.2-r0",
          "PkgName": "libssl3",
          "PkgIdentifier": {
            "PURL": "pkg:apk/alpine/libssl3@3.3.2-r0?arch=aarch64\u0026distro=3.20.3",
            "UID": "c4a39ef718e71832"
          },
          "InstalledVersion": "3.3.2-r0",
          "FixedVersion": "3.3.2-r1",
          "Status": "fixed",
          "Layer": {
            "DiffID": "sha256:651d9022c23486dfbd396c13db293af6845731cbd098a5f5606db4bc9f5573e8"
          },
          "PrimaryURL": "https://avd.aquasec.com/nvd/cve-2024-9143",
          "DataSource": {
            "ID": "alpine",
            "Name": "Alpine Secdb",
            "URL": "https://secdb.alpinelinux.org/"
          },
          "Title": "openssl: Low-level invalid GF(2^m) parameters lead to OOB memory access",
          "Description": "Issue summary: Use of the low-level GF(2^m) elliptic curve APIs with untrusted\nexplicit values for the field polynomial can lead to out-of-bounds memory reads\nor writes.\n\nImpact summary: Out of bound memory writes can lead to an application crash or\neven a possibility of a remote code execution, however, in all the protocols\ninvolving Elliptic Curve Cryptography that we're aware of, either only \"named\ncurves\" are supported, or, if explicit curve parameters are supported, they\nspecify an X9.62 encoding of binary (GF(2^m)) curves that can't represent\nproblematic input values. Thus the likelihood of existence of a vulnerable\napplication is low.\n\nIn particular, the X9.62 encoding is used for ECC keys in X.509 certificates,\nso problematic inputs cannot occur in the context of processing X.509\ncertificates.  Any problematic use-cases would have to be using an \"exotic\"\ncurve encoding.\n\nThe affected APIs include: EC_GROUP_new_curve_GF2m(), EC_GROUP_new_from_params(),\nand various supporting BN_GF2m_*() functions.\n\nApplications working with \"exotic\" explicit binary (GF(2^m)) curve parameters,\nthat make it possible to represent invalid field polynomials with a zero\nconstant term, via the above or similar APIs, may terminate abruptly as a\nresult of reading or writing outside of array bounds.  Remote code execution\ncannot easily be ruled out.\n\nThe FIPS modules in 3.3, 3.2, 3.1 and 3.0 are not affected by this issue.",
          "Severity": "LOW",
          "CweIDs": [
            "CWE-787"
          ],
          "VendorSeverity": {
            "amazon": 3,
            "redhat": 1,
            "ubuntu": 1
          },
          "CVSS": {
            "redhat": {
              "V3Vector": "CVSS:3.1/AV:N/AC:H/PR:N/UI:N/S:U/C:N/I:N/A:L",
              "V3Score": 3.7
            }
          },
          "References": [
            "https://access.redhat.com/security/cve/CVE-2024-9143",
            "https://github.com/openssl/openssl/commit/72ae83ad214d2eef262461365a1975707f862712",
            "https://github.com/openssl/openssl/commit/bc7e04d7c8d509fb78fc0e285aa948fb0da04700",
            "https://github.com/openssl/openssl/commit/c0d3e4d32d2805f49bec30547f225bc4d092e1f4",
            "https://github.com/openssl/openssl/commit/fdf6723362ca51bd883295efe206cb5b1cfa5154",
            "https://github.openssl.org/openssl/extended-releases/commit/8efc0cbaa8ebba8e116f7b81a876a4123594d86a",
            "https://github.openssl.org/openssl/extended-releases/commit/9d576994cec2b7aa37a91740ea7e680810957e41",
            "https://nvd.nist.gov/vuln/detail/CVE-2024-9143",
            "https://openssl-library.org/news/secadv/20241016.txt",
            "https://www.cve.org/CVERecord?id=CVE-2024-9143"
          ],
          "PublishedDate": "2024-10-16T17:15:18.13Z",
          "LastModifiedDate": "2024-11-08T16:35:21.58Z"
        }
      ]
    }
  ]
}"""

type TrivyReport =
    JsonProvider<Sample=sample, SampleIsList=true, ResolutionFolder=ResolutionFolder, RootName="TrivyReport">

type Vulnerability =
    { Title: string
      VulnerabilityID: string
      Severity: string
      Package: string
      Target: string
      InstalledVersion: string
      FixedVersion: string }

let vulnerabilities = new List<Vulnerability>()

let args: string array = fsi.CommandLineArgs
let fileName = args.[1] // Idx 0 is for the script name
let trivyResult = System.IO.File.ReadAllText fileName

TrivyReport.Parse(trivyResult).Results
|> Array.filter (fun result -> result.Vulnerabilities.Length > 0)
|> Array.iter (fun result ->
    result.Vulnerabilities
    |> Array.iter (fun vuln ->
        vulnerabilities.Add(
            { Title = vuln.Title
              VulnerabilityID = vuln.VulnerabilityId
              Severity = vuln.Severity
              Package = vuln.PkgName
              Target = result.Target
              InstalledVersion = vuln.InstalledVersion
              FixedVersion = vuln.FixedVersion }
        )))

module Summary =

    let private getVulnCount (vulnerabilitiesSeq: Vulnerability seq) (severity: string) =
        vulnerabilitiesSeq
        |> Seq.countBy (fun v -> v.Severity = severity)
        |> Seq.filter (fun q -> (fst q) = true)
        |> Seq.map snd
        |> Seq.tryHead
        |> function
            | Some x -> x
            | None -> 0

    let main () =
        let highVulnCounter = getVulnCount (vulnerabilities) ("HIGH")
        let criticalVulnCounter = getVulnCount (vulnerabilities) ("CRITICAL")

        printfn
            $"""## Trivy scan summary

Quantity of vulnerabilities found by severity:

| CRITICAL | HIGH |
|----------|------|
| {criticalVulnCounter} | {highVulnCounter} |
"""

module TableInfo =

    let private headerTarget (target: string) =
        $"<h3>Target: <code>{target}</code></h3>"

    let private tableHeader =
        """
| Package | ID | Severity | Installed Version | Fixed Version |
|---------|----|----------|-------------------|---------------|"""

    let private tableRow (vuln: Vulnerability) =
        $"| {vuln.Package} | {vuln.VulnerabilityID} | {vuln.Severity} | {vuln.InstalledVersion} | {vuln.FixedVersion} |"

    let main () =
        vulnerabilities
        |> Seq.groupBy _.Target
        |> Seq.fold
            (fun acc (t, vSeq) ->
                let header = headerTarget t

                let rows = vSeq |> Seq.fold (fun acc v -> $"{acc}{(tableRow v)}\n") ""

                $"{acc}{header}\n{tableHeader}\n{rows}\n")
            ""
        |> printfn "%s"

Summary.main ()
TableInfo.main ()
