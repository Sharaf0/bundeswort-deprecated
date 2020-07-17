import React, { useState } from 'react';
import { Form, FormGroup, Label, Input, Button } from 'reactstrap';
import Axios from 'axios';
import swal from 'sweetalert';
import YouTube from "react-youtube";
import RingLoader from "react-spinners/RingLoader";
import Highlighter from 'react-highlight-words';

class VideoResult {
  videoId: string;
  from: number;
  to: number;
  text: string;

  constructor() {
    this.videoId = '';
    this.from = 0;
    this.to = 0;
    this.text = '';
  }
}

export function Search() {
  const [searchQuery, setSearchQuery] = useState<string>('');
  const [videos, setVideos] = useState<VideoResult[]>(new Array<VideoResult>());
  const [currentVideoIndex, setCurrentVideoIndex] = useState<number>(-1);
  const [loading, setLoading] = useState<boolean>(false);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    try {
      setLoading(true);
      const res = await Axios.get<VideoResult[]>(`api/search/${searchQuery}`);
      setVideos(res.data);
      setCurrentVideoIndex(0);
    } catch (error) {
      swal({ text: 'Some error happened', timer: 5000 });
    } finally {
      setLoading(false);
    }
  }

  const changed = function (event: { target: any; data: number; }) {
    if (event.data === YouTube.PlayerState.ENDED || event.data === YouTube.PlayerState.PAUSED) {
      if (currentVideoIndex + 1 < videos.length)
        setCurrentVideoIndex(currentVideoIndex + 1);
    }
  }

  const currentVideo = videos[currentVideoIndex];

  const renderedVideo = currentVideo ?
    <div>
      <h3>{currentVideoIndex + 1} / {videos.length}</h3>
      <YouTube
        onStateChange={changed}
        videoId={currentVideo.videoId}
        opts={{ width: "100%", playerVars: { autoplay: 1, start: currentVideo.from, end: currentVideo.to, cc_load_policy: 1 } }}>
      </YouTube>
      <div>
        <Highlighter
          searchWords={[searchQuery]}
          autoEscape={true}
          textToHighlight={currentVideo.text}
        />
      </div>
    </div> : <></>;

  return loading ? <RingLoader
    size={150}
    color={"#123abc"}
    loading={true}
  /> : (
      <>
        <Form>
          <FormGroup>
            <Label><h1>Search Query:</h1></Label>
            <div style={{ display: "flex" }}>
              <Input style={{ width: "90%" }} type="text" placeholder="Aufruf" value={searchQuery} onChange={(e) => setSearchQuery(e.target.value)} />
              <Button style={{ width: "10%" }} type="submit" color="primary" onClick={handleSubmit}>Save</Button>
            </div>
          </FormGroup>
        </Form>
        {renderedVideo}
      </>
    );
}
