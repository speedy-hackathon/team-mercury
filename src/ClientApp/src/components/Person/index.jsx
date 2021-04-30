import React from "react";
import styles from "./styles.module.css";
import {MAX_HEIGHT, MAX_WIDTH} from "../../consts/sizes";

export default function Person({person, onClick, withInfectionRadius}) {
    const x = (person.position.x / MAX_WIDTH) * 100;
    const y = (person.position.y / MAX_HEIGHT) * 100;
    const isIll = person.healthStatus === 'Ill';
    return (
      <div className={styles.root} style={{left: `${x}%`, top: `${y}%`}}>
        <div className={withInfectionRadius && isIll ? styles.infectionRadius : ""} />
        <div className={`${styles.person} ${styles[person.healthStatus.toLowerCase()]} ${person.isBored ? styles.bored : null}`}
        onClick={() => onClick(person.id)} />
      </div>
    );
}

