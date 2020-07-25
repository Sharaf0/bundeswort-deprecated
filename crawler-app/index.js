if(process.argv.length < 5) {
    throw Error("too little arguments!")
}
const salutation = process.argv[2] === "M" ? "geehrter Herr" : "geehrte Frau";
const lastname = process.argv[3];
const address = process.argv[4];

const template = `Sehr ${salutation} ${lastname},

ich interessiere mich äußerst für Ihre Wohnung (${address}). Ab 01.09.2020 werde ich bei fair parken GmbH (Grafenberger Allee 337c 40235) als Software Entwickler arbeiten.

Die Wohnungsausstattung ist für mich optimal und auch der Grundriss sagt mir mit seiner praktischen Aufteilung sehr zu. Der Wohnort hat auch durch die relativ Nähe zur meiner Arbeitsstelle mein Interesse geweckt. Ich würde mich freuen, die Wohnung baldmöglichst besichtigen zu können.

Herzlichen Dank für Ihre Antwort im Voraus.

Mit besten Grüßen,
Mohammad Sharaf 0152 082 187 13 (erreichbar am besten von 13:00 bis 18:00 Uhr)
mohammedsharaf.1992@gmail.com`;

console.info(template);